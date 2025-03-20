using Data_Center.Configuration;
using Data_Center.Configuration.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StorageService.Exceptions;
using StorageService.Extensions;
using StorageService.Service.Interface;
using StorageService.StorageConstants;

namespace StorageService.Service;

public class SaveImageFileService : ISaveFile
{
    private readonly FileStorageOptions _options;
    private readonly ILogger<SaveImageFileService> _logger;
    private readonly IProgressNotifier _progressNotifier;

    #region Ctor

    public SaveImageFileService(
        IOptions<FileStorageOptions> options, 
        ILogger<SaveImageFileService> logger,
        IProgressNotifier progressNotifier)
    {
        _options = options.Value;
        _logger = logger;
        _progressNotifier = progressNotifier;
    }

    #endregion
    
    public IEnumerable<FileType> FileTypes => new FileType().GetImageFileTypes();

    public FolderType FolderType => FolderType.Images;

    public async Task<FileResultGeneric<FileMetadata>> SaveFileAsync(IFormFile file, string connectionId)
    {
        _logger.LogInformation($"{nameof(SaveImageFileService)} - SaveFileAsync. Saving image file {file.FileName}");
        long totalRead = 0; //Total read of file to be uploaded used in the progress report
        
        try
        {
            var folder = Path.Combine(_options.StoragePath, this.FolderType.ToString());
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var filePath = Path.Combine(folder, file.FileName);

            // Stream the file to the target location
            using (var targetStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, Constants.ImageFileBufferSize, useAsync: true))
            using (var sourceStream = file.OpenReadStream())
            {
                // Buffer size of chunk
                byte[] buffer = new byte[Constants.ImageFileBufferSize];
                int bytesRead;

                // while you haven't read the entire file.
                while ((bytesRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await targetStream.WriteAsync(buffer, 0, bytesRead);
                    totalRead += bytesRead; // Increase the total read of file

                    var progressPercentage = (int)((totalRead * 100) / file.Length);

                    await _progressNotifier.ReportProgressAsync(connectionId, progressPercentage);
                }
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
            _logger.LogError(ex, $"{nameof(SaveImageFileService)} - SaveFileAsync failed. {ex.Message}");
            throw new StorageException<FileMetadata>($"{nameof(SaveImageFileService)} Failed to save file: {ex.Message}, Stack Trace: {ex.StackTrace}");
        }
    }
}