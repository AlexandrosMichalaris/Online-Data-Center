using Microsoft.Extensions.Logging;
using StorageService.Exceptions;
using StorageService.Service.Interface;

namespace StorageService.Service;

public class RecoverFileService : IRecoverFileService
{
    private readonly ILogger<DeleteFileService> _logger;

    #region Ctor

    public RecoverFileService(ILogger<DeleteFileService> logger)
    {
        _logger = logger;
    }

    #endregion
    
    /// <summary>
    /// Move file from trash folder to original folder
    /// </summary>
    /// <param name="filepath">File path of original folder, as in the db is not changed.</param>
    /// <returns></returns>
    public async Task<FileResultGeneric<string>> RecoverFileAsync(string filepath)
    {
        try
        {
            var (fileName, trashFolder) = StorageHelper.GetFileNameAndTrashFolder(filepath);
            var trashFilePath = Path.Combine(trashFolder, fileName);
            
            // If file exists in trash
            if (!File.Exists(trashFilePath))
            {
                _logger.LogError($"{nameof(RecoverFileService)} - File in trash doesn't exist: {filepath}.");
                return FileResultGeneric<string>.Failure($"File in trash doesn't exist: {filepath}.");
            }
            
            File.Move(trashFilePath, filepath, true); // true will overwrite if the file exists
            
            return FileResultGeneric<string>.Success(trashFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(RecoverFileService)} - RecoverFileAsync - File recover failed. {ex.Message}, Stack Trace: {ex.StackTrace}");
            throw new StorageException<FileMetadata>($"{nameof(RecoverFileService)} - RecoverFileAsync - Failed to recover file: {ex.Message}, Stack Trace: {ex.StackTrace}");
        }
    }
}