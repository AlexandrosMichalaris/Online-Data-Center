namespace StorageService.Service.Interface;

public interface IDownloadService
{
    public Task<FileResultGeneric<Stream>> DownloadFileAsync(string filePath);
    
    public Task<FileResultGeneric<Stream>> PreviewFileAsync(string filePath);
}