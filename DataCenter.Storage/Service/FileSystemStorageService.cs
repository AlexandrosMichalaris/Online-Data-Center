using Data_Center.Configuration;
using DataCenter.Domain.Domain;
using Microsoft.Extensions.Logging;
using StorageService.Service.Interface;

namespace StorageService.Service;

public class FileSystemStorageService : IStorageService
{
    private readonly ILogger<FileSystemStorageService> _logger;
    private readonly FileStorageOptions _options;

    public FileSystemStorageService(ILogger<FileSystemStorageService> logger, FileStorageOptions options)
    {
        _logger = logger;
        _options = options;
    }

    public async Task SaveChunkAsync(FileChunk chunk, CancellationToken cancellationToken)
    {
        try
        {
            // Save chunks under folderType/chunks/
            var baseFolder = Path.Combine(_options.StoragePath, chunk.FileType.ToString(), "chunks");
            
            if (!Directory.Exists(baseFolder))
            {
                Directory.CreateDirectory(baseFolder);
            }

            var chunkPath = Path.Combine(baseFolder, $"{chunk.FileId}.chunk.{chunk.ChunkNumber}");

            await File.WriteAllBytesAsync(chunkPath, chunk.Data, cancellationToken);

            _logger.LogInformation("Saved chunk {ChunkNumber}/{TotalChunks} for file {FileId} at {Path}",
                chunk.ChunkNumber, chunk.TotalChunks, chunk.FileId, chunkPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save chunk {ChunkNumber} for file {FileId}", chunk.ChunkNumber, chunk.FileId);
            throw;
        }
    }

    public async Task ReassembleAsync(Guid fileId, int bufferSize, int totalChunks, string chunksFolder, string finalPath, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting reassembly for FileId={fileId}, totalChunks={totalChunks}", 
                fileId, totalChunks);

            await using (var finalStream = new FileStream(finalPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, useAsync: true))
            {
                for (var i = 0; i < totalChunks; i++)
                {
                    var chunkPath = Path.Combine(chunksFolder, $"{fileId}.chunk.{i}");

                    if (!File.Exists(chunkPath))
                    {
                        _logger.LogError("Missing chunk {ChunkIndex} for fileId={fileId}", i, fileId);
                        throw new FileNotFoundException($"Missing chunk {i}", chunkPath);
                    }

                    await using var chunkStream = new FileStream(
                        chunkPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 81920, useAsync: true);

                    await chunkStream.CopyToAsync(finalStream, cancellationToken);
                    _logger.LogDebug("Successfully appended chunk {ChunkIndex} for fileId={fileId}", i, fileId);
                }
            }
            
            _logger.LogInformation("File reassembly completed for fileId={fileId}. Final path: {FinalPath}", 
                fileId, finalPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reassemble file fileId={fileId}", fileId);
            throw; // bubble up for upper layers to handle (retry, DLQ, etc.)
        }
    }

    public Task CleanUpAsync(Guid fileId, int totalChunks, string chunksFolder,  CancellationToken cancellationToken)
    {
        try
        {
            // Cleanup after successful reassembly
            for (var i = 0; i < totalChunks; i++)
            {
                var chunkPath = Path.Combine(chunksFolder, $"{fileId}.chunk.{i}");
                if (File.Exists(chunkPath))
                {
                    File.Delete(chunkPath);
                    _logger.LogDebug("Deleted chunk {ChunkIndex} for FileId={FileId}", i, fileId);
                }
            }
            
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cleanup chunks for file FileId={FileId}", fileId);
            throw; // bubble up for upper layers to handle (retry, DLQ, etc.)
        }
    }
}