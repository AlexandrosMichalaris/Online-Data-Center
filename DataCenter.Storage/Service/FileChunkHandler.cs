using DataCenter.Domain.Domain;
using DataCenter.Domain.Queues;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using QueueMessageManagement.Interfaces;
using StorageService.Extensions;
using StorageService.Service.Interface;

namespace StorageService.Service;

public class FileChunkHandler : IFileChunkHandler
{
    private readonly IMessageProducer _producer;
    private readonly IProgressNotifier _progressNotifier;
    private readonly ILogger<FileChunkHandler> _logger;

    #region Ctor

    public FileChunkHandler(IMessageProducer producer, IProgressNotifier progressNotifier, ILogger<FileChunkHandler> logger)
    {
        _producer = producer;
        _progressNotifier = progressNotifier;
        _logger = logger;
    }

    #endregion
    
    public async Task ChunkProducerAsync(
        IFormFile file, 
        Guid fileId, 
        string connectionId,
        CancellationToken cancellationToken = default)
    {
        var fileType = FileTypeMapper.GetFileTypeFromContentType(file.ContentType);
        var bufferSize = StorageHelper.GetBufferSizeFromFileType(fileType);
        var totalChunks = (int)Math.Ceiling((double)file.Length / bufferSize);

        // Wraps the whole file-processing flow
        try
        {
            _logger.LogInformation("Starting chunk production for FileId={FileId}, FileName={FileName}, TotalChunks={TotalChunks}", 
                fileId, file.FileName, totalChunks);

            await using var stream = file.OpenReadStream();

            var chunksSent = 0;

            await foreach (var (chunkData, chunkIndex) in stream.ReadChunksAsync(bufferSize, cancellationToken))
            {
                // Inner try catch ensures exactly which chunk failed, without losing context
                try
                {
                    var chunk = new FileChunk
                    {
                        FileId = fileId,
                        ChunkNumber = chunkIndex,
                        TotalChunks = totalChunks,
                        FileName = file.FileName,
                        Data = chunkData,
                        FileType = fileType
                    };

                    await _producer.SendAsync(Queues.FileUploadQueue, chunk, cancellationToken);

                    chunksSent++;
                    var progress = (int)((chunksSent * 100.0) / totalChunks);

                    // Notify progress via SignalR
                    await _progressNotifier.ReportProgressAsync(connectionId, progress);

                    _logger.LogDebug("Produced chunk {ChunkIndex}/{TotalChunks} for FileId={FileId}. Progress: {Progress}%", 
                        chunkIndex + 1, totalChunks, fileId, progress);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, 
                        "Failed to send chunk {ChunkIndex}/{TotalChunks} for FileId={FileId}, FileName={FileName}", 
                        chunkIndex, totalChunks, fileId, file.FileName);
                    throw; // bubble up for retry
                }
            }

            _logger.LogInformation("Completed chunk production for FileId={FileId}, FileName={FileName}", 
                fileId, file.FileName);

            // Final 100% notification
            await _progressNotifier.ReportProgressAsync(connectionId, 100);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Error during chunk production for FileId={FileId}, FileName={FileName}", 
                fileId, file.FileName);
            throw;
        }
    }
}