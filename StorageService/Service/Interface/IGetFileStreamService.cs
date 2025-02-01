namespace StorageService.Service.Interface;

public interface IGetFileStreamService
{
    public Task<FileResultGeneric<Stream>> GetFileStream(string filePath);
}