using Data_Center.Configuration.Constants;
using Microsoft.AspNetCore.Http;

namespace StorageService.Service.Interface;

public interface ISaveFile
{
    // TODO: Decorator to check each accepted size.
    public IEnumerable<FileType> FileTypes { get; }
    
    public FolderType FolderType { get; }
    
    public Task<FileResultGeneric<FileMetadata>> SaveFileAsync(IFormFile file);
    
    public Task<FileResultGeneric<Stream>> GetFileStream(string filePath);
}