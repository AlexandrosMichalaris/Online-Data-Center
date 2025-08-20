using Data_Center.Configuration.Constants;
using DataCenter.Domain.Domain;
using DataCenter.Domain.Dto;
using DataCenter.Domain.Queues;
using Microsoft.Extensions.Logging;
using QueueMessageManagement.Interfaces;
using QueueMessageManagment.Consumer;
using StackExchange.Redis;
using StorageService.Service.Interface;

namespace StorageService.Service;

public class FileChunkConsumer : ConsumerBase<FileChunk>
{
    private readonly ILogger<FileChunkConsumer> _logger;
    private readonly IStorageService _storage;
    private readonly IProducer _producer;
    private readonly IDatabase _redis; // from StackExchange.Redis

    public FileChunkConsumer(IProducer producer, IStorageService storage, ILogger<FileChunkConsumer> logger, IConnectionMultiplexer redis)
    {
        _producer = producer;
        _storage = storage;
        _logger = logger;
        _redis = redis.GetDatabase();
    }
    
    public override string QueueName => Queues.FileUploadQueue;

    public override async Task ExecuteAsync(FileChunk message, CancellationToken cancellationToken)
    {
        try
        {
            // Save the chunk to disk
            await _storage.SaveChunkAsync(message, cancellationToken);

            // Increment Redis counter
            var counterKey = $"file:{message.FileId}:chunks";

            var count = await _redis.StringIncrementAsync(counterKey);

            _logger.LogInformation(
                "Stored chunk {ChunkNumber}/{TotalChunks} for file {FileId}. Counter = {NewCount}",
                message.ChunkNumber, message.TotalChunks, message.FileId, count);

            // If this is the last chunk, try to acquire lock
            if (count == message.TotalChunks)
            {
                var gotLock = await _redis.StringSetAsync(
                    $"file:{message.FileId}:reassembly-lock",
                    "1",
                    TimeSpan.FromMinutes(1), // lock expiry
                    When.NotExists);

                if (gotLock)
                {
                    _logger.LogInformation("All chunks received for file {FileId}. Publishing reassembly request.",
                        message.FileId);

                    var reassembleMessage = new ReassembleFileMessage
                    {
                        FileId = message.FileId,
                        TotalChunks = message.TotalChunks,
                        FileType = message.FileType,
                        FolderType = message.FolderType,
                        FileName = message.FileName,
                    };

                    await _producer.SendAsync(Queues.ReassembleQueue, reassembleMessage, cancellationToken);
                }
                else
                {
                    _logger.LogWarning("Reassembly already triggered for file {FileId}. Skipping duplicate trigger.",
                        message.FileId);
                }
                
                // Removing lock happens in reassembly
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing chunk {ChunkNumber} for file {FileId}", message.ChunkNumber,
                message.FileId);
            throw; // rethrow so RabbitMQ can retry / DLQ
        }
    }
}