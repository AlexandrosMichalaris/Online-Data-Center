using Data_Center.Configuration;
using Data_Center.Configuration.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using StorageService.Exceptions;
using StorageService.Extensions;
using StorageService.Service.Interface;

namespace StorageService.Service;

public class SaveDocumentFileService : ISaveFile
{
    private readonly FileStorageOptions _options;

    public SaveDocumentFileService(IOptions<FileStorageOptions> options)
    {
        _options = options.Value;
    }
    
    public IEnumerable<FileType> FileTypes =>  new FileType().GetDocumentFileTypes();

    public FolderType FolderType => FolderType.Documents;
    
    public async Task<FileResultGeneric<FileMetadata>> SaveFileAsync(IFormFile file)
    {
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
                //Todo: Consider to change this into Promise.WhenAll()
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
        catch (Exception e)
        {
            throw new StorageException<FileMetadata>($"{typeof(SaveDocumentFileService)} Failed to save file: {e.Message}, Stack Trace: {e.StackTrace}");
        }
    }
}