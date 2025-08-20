using Data_Center.Configuration;
using DataCenter.Domain.Dto;
using DataCenter.Domain.Queues;
using Microsoft.Extensions.Logging;
using QueueMessageManagement.Interfaces;
using QueueMessageManagment.Consumer;

namespace StorageService.Service;

public class ReassembleFileChunkAsync : ConsumerBase<ReassembleFileMessage>
{
    private readonly FileStorageOptions _options;
    private readonly ILogger<ReassembleFileChunkAsync> _logger;

    public ReassembleFileChunkAsync(FileStorageOptions options, ILogger<ReassembleFileChunkAsync> logger)
    {
        _options = options;
        _logger = logger;
    }

    public override string QueueName => Queues.ReassembleQueue;
    
    public override async Task ExecuteAsync(ReassembleFileMessage message, CancellationToken cancellationToken)
    { 
        var baseFolder = Path.Combine(_options.StoragePath, message.FolderType.ToString());
        var finalPath = Path.Combine(baseFolder, $"{message.FileId}.final"); 
        var bufferSize = StorageHelper.GetBufferSizeFromFileType(message.FileType);

        try
        {
            _logger.LogInformation("Starting reassembly for FileId={FileId}, TotalChunks={TotalChunks}", 
                message.FileId, message.TotalChunks);

            await using (var finalStream = new FileStream(finalPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, useAsync: true))
            {
                for (var i = 0; i < message.TotalChunks; i++)
                {
                    var chunkPath = Path.Combine(baseFolder, $"{message.FileId}.chunk.{i}");

                    if (!File.Exists(chunkPath))
                    {
                        _logger.LogError("Missing chunk {ChunkIndex} for FileId={FileId}", i, message.FileId);
                        throw new FileNotFoundException($"Missing chunk {i}", chunkPath);
                    }

                    await using var chunkStream = new FileStream(
                        chunkPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 81920, useAsync: true);

                    await chunkStream.CopyToAsync(finalStream, cancellationToken);
                    _logger.LogDebug("Successfully appended chunk {ChunkIndex} for FileId={FileId}", i, message.FileId);
                }
            }

            // Cleanup after successful reassembly
            for (var i = 0; i < message.TotalChunks; i++)
            {
                var chunkPath = Path.Combine(baseFolder, $"{message.FileId}.chunk.{i}");
                if (File.Exists(chunkPath))
                {
                    File.Delete(chunkPath);
                    _logger.LogDebug("Deleted chunk {ChunkIndex} for FileId={FileId}", i, message.FileId);
                }
            }

            _logger.LogInformation("File reassembly completed for FileId={FileId}. Final path: {FinalPath}", 
                message.FileId, finalPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reassemble file FileId={FileId}", message.FileId);
            throw; // bubble up for upper layers to handle (retry, DLQ, etc.)
        }
    }
}