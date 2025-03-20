using Data_Center.Configuration;
using Data_Center.Configuration.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StorageService.Exceptions;
using StorageService.Extensions;
using StorageService.Service.Interface;
using StorageService.StorageConstants;

namespace StorageService.Service;

public class SaveDocumentFileService : ISaveFile
{
    private readonly ILogger<SaveDocumentFileService> _logger;
    private readonly FileStorageOptions _options;
    private readonly IProgressNotifier _progressNotifier;

    #region Ctor

    public SaveDocumentFileService(
        IOptions<FileStorageOptions> options, 
        ILogger<SaveDocumentFileService> logger
        , IProgressNotifier progressNotifier)
    {
        _options = options.Value;
        _logger = logger;
        _progressNotifier = progressNotifier;
    }

    #endregion
    
    public IEnumerable<FileType> FileTypes =>  new FileType().GetDocumentFileTypes();

    public FolderType FolderType => FolderType.Documents;
    
    public async Task<FileResultGeneric<FileMetadata>> SaveFileAsync(IFormFile file, string connectionId)
    {
        _logger.LogInformation($"{nameof(SaveDocumentFileService)} - SaveFileAsync. Saving document file {file.FileName}");
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
            using (var targetStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, Constants.DocumentFileBufferSize, useAsync: true))
            using (var sourceStream = file.OpenReadStream())
            {
                // Buffer size of chunk
                byte[] buffer = new byte[Constants.DocumentFileBufferSize];
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
            _logger.LogError(ex, $"{nameof(SaveDocumentFileService)} - SaveFileAsync failed. {ex.Message}");
            throw new StorageException<FileMetadata>($"{nameof(SaveDocumentFileService)} Failed to save file: {ex.Message}, Stack Trace: {ex.StackTrace}");
        }
    }
}