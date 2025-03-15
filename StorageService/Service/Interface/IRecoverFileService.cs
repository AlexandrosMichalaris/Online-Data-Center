namespace StorageService.Service.Interface;

public interface IRecoverFileService
{
    public Task<FileResultGeneric<string>> RecoverFileAsync(string filepath);
}