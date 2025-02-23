namespace StorageService.Service.Interface;

public interface IDeleteFileService
{
    public Task<FileResultGeneric<string>> DeleteFile(string filePath);
}