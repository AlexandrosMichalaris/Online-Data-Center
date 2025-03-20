using StorageService.Model;

namespace StorageService.Service.Interface;

public interface IDownloadService
{
    public Task<FileResultGeneric<StreamData>> DownloadFileAsync(int id);
    
    public Task<FileResultGeneric<StreamData>> DownloadMultipleFilesAsync(IEnumerable<int> ids);
    
    public Task<FileResultGeneric<Stream>> PreviewFileAsync(string filePath);
}