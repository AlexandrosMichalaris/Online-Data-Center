using Data_Center.Configuration.Constants;
using StorageService.Service.Interface;

namespace StorageService.Service.Strategy;

public class FileHandlerStrategy : IFileHandlerStrategy
{
    private readonly IEnumerable<ISaveFile> _handlers;

    public FileHandlerStrategy(IEnumerable<ISaveFile> handlers)
    {
        _handlers = handlers;
    }
    
    public ISaveFile GetFileHandler(FileType fileType)
    {
        var strategy = _handlers.SingleOrDefault(x => x.FileTypes.Contains(fileType));
        
        if(strategy is null)
            throw new ApplicationException("Invalid strategy type");
        
        return strategy;
    }
}