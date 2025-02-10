using Data_Center.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StorageService.Exceptions;
using StorageService.Service.Interface;

namespace StorageService.Service;

public class GetFileStreamServiceService : IGetFileStreamService
{
    private readonly ILogger<GetFileStreamServiceService> _logger;
    private readonly FileStorageOptions _options;

    #region Ctor

    public GetFileStreamServiceService(IOptions<FileStorageOptions> options, ILogger<GetFileStreamServiceService> logger)
    {
        _logger = logger;
        _options = options.Value;
    }

    #endregion

    public async Task<FileResultGeneric<Stream>> GetFileStreamAsync(string filePath)
    {
        _logger.LogInformation($"{typeof(GetFileStreamServiceService)} - GetFileStreamAsync - Starting for {filePath}.");
        
        try
        {
            if (!File.Exists(filePath))
            {
                return FileResultGeneric<Stream>.Failure("File not found.");
            }

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return FileResultGeneric<Stream>.Success(fileStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{typeof(GetFileStreamServiceService)} - GetFileStreamAsync failed. {ex.Message}");
            throw new StorageException<Stream>($"Failed to retrieve file: {ex.Message}");
        }
    }
}