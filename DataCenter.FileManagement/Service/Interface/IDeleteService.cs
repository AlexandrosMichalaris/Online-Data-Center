namespace StorageService.Service.Interface;

public interface IDeleteService
{
    public Task<FileResultGeneric<FileMetadata>> DeleteFileAsync(Guid id);
}