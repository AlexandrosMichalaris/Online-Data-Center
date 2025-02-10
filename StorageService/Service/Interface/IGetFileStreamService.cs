namespace StorageService.Service.Interface;

public interface IGetFileStreamService
{
    public Task<FileResultGeneric<Stream>> GetFileStreamAsync(string filePath);
}