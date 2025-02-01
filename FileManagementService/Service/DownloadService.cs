using StorageService.Exceptions;
using StorageService.Service.Interface;

namespace StorageService.Service;

public class DownloadService : IDownloadService
{
    public DownloadService()
    {
    }

    public Task<FileResultGeneric<Stream>> DownloadFileAsync(string filePath)
    {
        try
        {
            throw new System.NotImplementedException();
        }
        catch (StorageException<FileMetadata> ex)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new ApplicationException($"{typeof(UploadService)} Exception on Download File Service {e.Message}, Stack Trace: {e.StackTrace}");
        }
    }

    public Task<FileResultGeneric<Stream>> PreviewFileAsync(string filePath)
    {
        try
        {
            //Only for PDF Files
            throw new System.NotImplementedException();
        }
        catch (StorageException<FileMetadata> ex)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new ApplicationException($"{typeof(UploadService)} Exception on Preview File Service {e.Message}, Stack Trace: {e.StackTrace}");
        }
    }
}