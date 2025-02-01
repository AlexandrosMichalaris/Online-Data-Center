using StorageService.Service.Interface;

namespace StorageService.Service;

public class GetFileStreamService : IGetFileStream
{
    public GetFileStreamService()
    {
    }

    public Task<FileResultGeneric<Stream>> GetFileStream(string filePath)
    {
        throw new NotImplementedException();
    }
}