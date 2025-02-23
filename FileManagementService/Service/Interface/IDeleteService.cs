namespace StorageService.Service.Interface;

public interface IDeleteService
{
    public Task<FileResultGeneric<FileMetadata>> DeleteFileAsync(int id);
    
    public Task<FileResultGeneric<string>> RecoverFileAsync(int id);
}