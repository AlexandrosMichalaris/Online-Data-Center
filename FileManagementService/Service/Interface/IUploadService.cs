using Microsoft.AspNetCore.Http;

namespace StorageService.Service.Interface;

public interface IFileManagementService
{
    public Task<FileResultGeneric<FileMetadata>> UploadFileAsync(IFormFile file);
    
    public Task<FileResultGeneric<Stream>> PreviewFileAsync(string filePath);
}