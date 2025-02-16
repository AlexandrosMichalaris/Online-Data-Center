using Microsoft.AspNetCore.Http;

namespace StorageService.Service.Interface;

public interface IUploadService
{
    public Task<FileResultGeneric<FileMetadata>> UploadFileAsync(IFormFile file, string connectionId);
}