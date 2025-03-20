namespace StorageService.Service.Interface;

public interface IDeleteFileService
{
    public Task<FileResultGeneric<string>> DeleteFileAsync(string filePath);
    
    public Task<FileResultGeneric<string>> DeleteFileFromTrashAsync(string filePath);
    
    public Task<FileResultGeneric<string>> RecycleFileAsync(string filepath);
}