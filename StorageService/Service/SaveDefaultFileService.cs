using Data_Center.Configuration;
using Data_Center.Configuration.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StorageService.Exceptions;
using StorageService.Extensions;
using StorageService.Service.Interface;

namespace StorageService;

public class SaveDefaultFileService : ISaveFile
{
    private readonly ILogger<SaveDefaultFileService> _logger;
    private readonly FileStorageOptions _options;

    #region Ctor

    public SaveDefaultFileService(IOptions<FileStorageOptions> options, ILogger<SaveDefaultFileService> logger)
    {
        _logger = logger;
        _options = options.Value;
    }

    #endregion
    
    public IEnumerable<FileType> FileTypes => new FileType().GetDefaultFileTypes();

    public FolderType FolderType => FolderType.Others;

    public async Task<FileResultGeneric<FileMetadata>> SaveFileAsync(IFormFile file)
    {
        _logger.LogInformation($"{nameof(SaveDefaultFileService)} - SaveFileAsync. Saving general file {file.FileName}");
        
        try
        {
            var folder = Path.Combine(_options.StoragePath, this.FolderType.ToString());
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            
            var filePath = Path.Combine(folder, file.FileName);

            // Stream the file to the target location
            using (var targetStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
            using (var sourceStream = file.OpenReadStream())
            {
                await sourceStream.CopyToAsync(targetStream);
            }
            
            //Todo: Check size accordingly
            
            var mimeType = file.ContentType;

            var metadata = new FileMetadata(
                filePath: filePath,
                fileName: file.FileName,
                fileSize: file.Length,
                mimeType: mimeType,
                uploadTime: DateTime.UtcNow,
                storageFolder: folder
            );

            return FileResultGeneric<FileMetadata>.Success(metadata);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(SaveDefaultFileService)} - SaveFileAsync failed. {ex.Message}");
            throw new StorageException<FileMetadata>($"{nameof(SaveDefaultFileService)} Failed to save file: {ex.Message}, Stack Trace: {ex.StackTrace}");
        }
    }
}