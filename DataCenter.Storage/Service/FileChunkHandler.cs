using DataCenter.Domain.Domain;
using DataCenter.Domain.Queues;
using Microsoft.AspNetCore.Http;
using QueueMessageManagement.Interfaces;
using StorageService.Extensions;
using StorageService.Service.Interface;

namespace StorageService.Service;

public class FileChunkHandler : IFileChunkHandler
{
    private readonly IMessageProducer _producer;
    private readonly IProgressNotifier _progressNotifier;
    //private readonly

    #region Ctor

    public FileChunkHandler(IMessageProducer producer, IProgressNotifier progressNotifier)
    {
        _producer = producer;
        _progressNotifier = progressNotifier;
    }

    #endregion
    
    public async Task ChunkProducerAsync(IFormFile file, Guid fileId, string connectionString, CancellationToken cancellationToken = default)
    {
        var fileType = FileTypeMapper.GetFileTypeFromContentType(file.ContentType);
        
        var bufferSize = StorageHelper.GetBufferSizeFromFileType(fileType);
        
        var totalChunks = (int)Math.Ceiling((double)file.Length / bufferSize);

        await using var stream = file.OpenReadStream();

        await foreach (var (chunkData, chunkIndex) in stream.ReadChunksAsync(bufferSize,
                           cancellationToken: cancellationToken))
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
        }
    }
}