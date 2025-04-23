using StorageService.Model;

namespace StorageService.Service.Interface;

public interface IDownloadService
{
    public Task<FileResultGeneric<StreamData>> DownloadFileAsync(Guid id);
    
    public Task<FileResultGeneric<StreamData>> DownloadMultipleFilesAsync(IEnumerable<Guid> ids);
    
    public Task<FileResultGeneric<Stream>> PreviewFileAsync(string filePath);
}