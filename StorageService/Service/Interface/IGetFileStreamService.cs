namespace StorageService.Service.Interface;

public interface IGetFileStream
{
    public Task<FileResultGeneric<Stream>> GetFileStream(string filePath);
}