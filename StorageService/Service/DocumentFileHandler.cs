using Data_Center.Configuration.Constants;
using Microsoft.AspNetCore.Http;
using StorageService.Service.Interface;

namespace StorageService.Service;

public class DocumentFileHandler : ISaveFile
{
    public IEnumerable<FileType> FileType { get; }
    
    public async Task<FileStorageResult<FileMetadata>> SaveFileAsync(IFormFile file, string basePath)
    {
        try
        {
            var folder = Path.Combine(basePath, "Others");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(folder, fileName);

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

            return FileStorageResult<FileMetadata>.Success(metadata);
        }
        catch (Exception e)
        {
            return FileStorageResult<FileMetadata>.Failure($"Failed to save file: {e.Message}");
        }
    }
}