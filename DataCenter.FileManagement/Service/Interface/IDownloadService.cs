using StorageService.Model;

namespace StorageService.Service.Interface;

public interface IDownloadService
{
    Task<FileResultGeneric<StreamData>> DownloadFileAsync(Guid id);
    
    Task<FileResultGeneric<StreamData>> DownloadMultipleFilesAsync(IEnumerable<Guid> ids);
    
    Task<FileResultGeneric<Stream>> PreviewFileAsync(string filePath);
}