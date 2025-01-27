using Data_Center.Configuration;
using Data_Center.Configuration.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using StorageService.Exceptions;
using StorageService.Extensions;
using StorageService.Service.Interface;

namespace StorageService;

public class DefaultFileHandler : ISaveFile
{
    private readonly FileStorageOptions _options;

    public DefaultFileHandler(IOptions<FileStorageOptions> options)
    {
        _options = options.Value;
    }
    
    public IEnumerable<FileType> FileTypes => new FileType().GetDefaultFileTypes();

    public FolderType FolderType => FolderType.Others;

    public async Task<FileResultGeneric<FileMetadata>> SaveFileAsync(IFormFile file)
    {
        try
        {
            var folder = Path.Combine(_options.StoragePath, this.FolderType.ToString());
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
            throw new StorageException<FileMetadata>($"{typeof(DefaultFileHandler)} Failed to save file: {e.Message}");
        }
    }

    public async Task<FileResultGeneric<Stream>> GetFileStream(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return FileResultGeneric<Stream>.Failure("File not found.");
            }

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return FileResultGeneric<Stream>.Success(fileStream);
        }
        catch (Exception ex)
        {
            throw new StorageException<Stream>($"Failed to retrieve file: {ex.Message}");
        }
    }
}