using Microsoft.Extensions.Logging;
using StorageService.Exceptions;
using StorageService.Service.Interface;

namespace StorageService.Service;

public class DownloadService : IDownloadService
{
    private readonly ILogger<DownloadService> _logger;

    #region Ctor

    public DownloadService(ILogger<DownloadService> logger)
    {
        _logger = logger;
    }

    #endregion

    public Task<FileResultGeneric<Stream>> DownloadFileAsync(string filePath)
    {
        try
        {
            throw new System.NotImplementedException();
        }
        catch (StorageException<FileMetadata> ex)
        {
            _logger.LogError(ex, $"{typeof(DownloadService)} - DownloadFileAsync - Storage Exception on download. {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{typeof(DownloadService)} - DownloadFileAsync failed. {ex.Message}");
            throw new ApplicationException($"{typeof(DownloadService)} Exception on Download File Service {ex.Message}, Stack Trace: {ex.StackTrace}");
        }
    }

    public Task<FileResultGeneric<Stream>> PreviewFileAsync(string filePath)
    {
        try
        {
            //Only for PDF Files
            throw new System.NotImplementedException();
        }
        catch (StorageException<FileMetadata> ex)
        {
            _logger.LogError(ex, $"{typeof(DownloadService)} - PreviewFileAsync - Storage Exception on preview. {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{typeof(DownloadService)} - PreviewFileAsync failed. {ex.Message}");
            throw new ApplicationException($"{typeof(DownloadService)} Exception on Preview File Service {ex.Message}, Stack Trace: {ex.StackTrace}");
        }
    }
}