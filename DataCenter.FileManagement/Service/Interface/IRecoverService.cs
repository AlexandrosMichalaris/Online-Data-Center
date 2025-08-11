namespace StorageService.Service.Interface;

public interface IRecoverService
{
    Task<FileResultGeneric<FileMetadata>> RecoverFileAsync(Guid id);
}