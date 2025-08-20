using Data_Center.Configuration;
using DataCenter.Domain.Dto;
using DataCenter.Domain.Queues;
using Microsoft.Extensions.Logging;
using QueueMessageManagement.Interfaces;
using QueueMessageManagment.Consumer;
using StackExchange.Redis;
using StorageService.Service.Interface;

namespace StorageService.Service;

public class ReassembleConsumer : ConsumerBase<ReassembleFileMessage>
{
    private readonly FileStorageOptions _options;
    private readonly ILogger<ReassembleConsumer> _logger;
    private readonly IStorageService _storageService;
    private readonly IDatabase _redis; // from StackExchange.Redis

    public ReassembleConsumer(FileStorageOptions options, ILogger<ReassembleConsumer> logger, IStorageService storageService, IConnectionMultiplexer redis)
    {
        _options = options;
        _logger = logger;
        _storageService = storageService;
        _redis = redis.GetDatabase();
    }

    public override string QueueName => Queues.ReassembleQueue;
    
    public override async Task ExecuteAsync(ReassembleFileMessage message, CancellationToken cancellationToken)
    {
        var folderTypePath = Path.Combine(_options.StoragePath, message.FolderType.ToString());
        var chunksFolder = Path.Combine(folderTypePath, "chunks");
        var finalPath = Path.Combine(folderTypePath, message.FileName);
        var bufferSize = StorageHelper.GetBufferSizeFromFileType(message.FileType);
        var lockKey = $"file:{message.FileId}:lock";
        
        try
        {
            await _storageService.ReassembleAsync(message.FileId, bufferSize, message.TotalChunks, chunksFolder, finalPath, cancellationToken);
        
            await _redis.KeyDeleteAsync(lockKey);
            _logger.LogInformation("Lock released for FileId={FileId}", message.FileId);
        
            await _storageService.CleanUpAsync(message.FileId, message.TotalChunks, chunksFolder, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reassemble file FileId={FileId}", message.FileId);
            // optionally: you might want to release the lock here too if you want retries
            throw;
        }
    }
}