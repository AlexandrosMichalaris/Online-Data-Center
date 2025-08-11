using Microsoft.AspNetCore.Http;

namespace StorageService.Service.Interface;

public interface IUploadService
{
    Task<FileResultGeneric<FileMetadata>> UploadFileAsync(IFormFile file, string connectionId);
}