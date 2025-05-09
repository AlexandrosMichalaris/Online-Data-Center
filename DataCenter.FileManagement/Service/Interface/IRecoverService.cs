namespace StorageService.Service.Interface;

public interface IRecoverService
{
    public Task<FileResultGeneric<FileMetadata>> RecoverFileAsync(Guid id);
}