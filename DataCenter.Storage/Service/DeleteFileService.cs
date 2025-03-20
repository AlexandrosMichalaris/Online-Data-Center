using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using StorageService.Exceptions;
using StorageService.Service.Interface;
using Constants = StorageService.StorageConstants.Constants;

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
    public async Task<FileResultGeneric<string>> DeleteFileAsync(string filePath)
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

    public async Task<FileResultGeneric<string>> DeleteFileFromTrashAsync(string filePath)
    {
        var (fileName, trashFolder) = StorageHelper.GetFileNameAndTrashFolder(filePath);
        
        return await this.DeleteFileAsync(StorageHelper.Combine(trashFolder, fileName));
    }


    public async Task<FileResultGeneric<string>> RecycleFileAsync(string filepath)
    {
        try
        {
            if (!File.Exists(filepath))
            {
                _logger.LogError($"{nameof(DeleteFileService)} - DeleteFile - Filepath does not exist: {filepath}.");
                return FileResultGeneric<string>.Failure($"Filepath doesn't exist: {filepath}.");
            }
            
            var (fileName, trashFolder) = StorageHelper.GetFileNameAndTrashFolder(filepath);

            // Ensure the trash folder exists
            if (!Directory.Exists(trashFolder))
            {
                Directory.CreateDirectory(trashFolder);
            }

            // Create the new file path inside the trash folder
            var trashFilePath = Path.Combine(trashFolder, fileName);

            // Move the file to the trash folder
            File.Move(filepath, trashFilePath, true); // true will overwrite if the file exists
        
            return FileResultGeneric<string>.Success(trashFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(DeleteFileService)} - DeleteFile - File deletion failed. {ex.Message}, Stack Trace: {ex.StackTrace}");
            throw new StorageException<FileMetadata>($"{nameof(DeleteFileService)} - DeleteFile - Failed to delete file: {ex.Message}, Stack Trace: {ex.StackTrace}");
        }
    }
}