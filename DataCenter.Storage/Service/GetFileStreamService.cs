using Data_Center.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StorageService.Exceptions;
using StorageService.Service.Interface;

namespace StorageService.Service;

public class GetFileStreamService : IGetFileStreamService
{
    private readonly ILogger<GetFileStreamService> _logger;
    private readonly FileStorageOptions _options;

    #region Ctor

    public GetFileStreamService(IOptions<FileStorageOptions> options, ILogger<GetFileStreamService> logger)
    {
        _logger = logger;
        _options = options.Value;
    }

    #endregion

    public async Task<FileResultGeneric<Stream>> GetFileStreamAsync(string filePath)
    {
        _logger.LogInformation($"{nameof(GetFileStreamService)} - GetFileStreamAsync - Starting for {filePath}.");
        
        try
        {
            if (!File.Exists(filePath))
            {
                _logger.LogError($"{nameof(GetFileStreamService)} - GetFileStreamAsync - File {filePath} does not exist.");
                return FileResultGeneric<Stream>.Failure("File not found.");
            }

            // We use useAsync: true for non-blocking file access.
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
            return FileResultGeneric<Stream>.Success(fileStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetFileStreamService)} - GetFileStreamAsync failed. {ex.Message}");
            throw new StorageException<Stream>($"Failed to retrieve file: {ex.Message}");
        }
    }
}