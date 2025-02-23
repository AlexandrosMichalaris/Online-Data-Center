using Microsoft.Extensions.Logging;
using StorageService.Exceptions;
using StorageService.Service.Interface;

namespace StorageService.Service;

public class DeleteFileService : IDeleteFileService
{
    private readonly ILogger<DeleteFileService> _logger;

    #region Ctor

    public DeleteFileService(ILogger<DeleteFileService> logger)
    {
        _logger = logger;
    }

    #endregion
    
    
    /// <summary>
    /// Deletes a file from the specified path asynchronously.
    /// </summary>
    /// <param name="filePath">The full path to the file to be deleted.</param>
    public async Task<FileResultGeneric<string>> DeleteFile(string filePath)
    {
        try
        {
            _logger.LogInformation($"{nameof(DeleteFileService)} - DeleteFile - File deletion {filePath} started.");

            if (!File.Exists(filePath))
            {
                _logger.LogError($"{nameof(DeleteFileService)} - DeleteFile - Filepath does not exist: {filePath}.");
                return FileResultGeneric<string>.Failure($"Filepath doesn't exist: {filePath}.");
            }

            File.Delete(filePath);
            
            return FileResultGeneric<string>.Success(filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(DeleteFileService)} - DeleteFile - File deletion failed. {ex.Message}, Stack Trace: {ex.StackTrace}");
            throw new StorageException<FileMetadata>($"{nameof(DeleteFileService)} Failed to delete file: {ex.Message}, Stack Trace: {ex.StackTrace}");
        }
    }
}