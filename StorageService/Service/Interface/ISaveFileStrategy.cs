using Data_Center.Configuration.Constants;

namespace StorageService.Service.Interface;

public interface ISaveFileStrategy
{
    ISaveFile GetFileHandler(FileType fileType);
}