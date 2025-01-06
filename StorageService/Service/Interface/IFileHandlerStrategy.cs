using Data_Center.Configuration.Constants;

namespace StorageService.Service.Interface;

public interface IFileHandlerStrategy
{
    ISaveFile GetFileHandler(FileType fileType);
}