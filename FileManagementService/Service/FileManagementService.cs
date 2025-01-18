using Microsoft.AspNetCore.Http;
using StorageService.Exceptions;
using StorageService.Repository.Interface;
using StorageService.Service.Interface;

namespace StorageService.Service;

public class FileManagementService : IFileManagementService
{
    private readonly IFileHandlerStrategy _fileHandlerStrategy;
    private readonly IFileRecordRepository _fileRecordRepository;
    
    public FileManagementService(
        IFileHandlerStrategy fileHandlerStrategy,
        IFileRecordRepository fileRecordRepository
        )
    {
        _fileHandlerStrategy = fileHandlerStrategy;
        _fileRecordRepository = fileRecordRepository;
    }
    
    public Task<FileResultGeneric<FileMetadata>> UploadFileAsync(IFormFile file)
    {
        try
        {

        }
        catch (StorageException<FileMetadata> ex)
        {
            throw;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Task<FileResultGeneric<Stream>> DownloadFileAsync(string filePath)
    {
        try
        {

        }
        catch (StorageException<FileMetadata> ex)
        {
            throw;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Task<FileResultGeneric<Stream>> PreviewFileAsync(string filePath)
    {
        try
        {

        }
        catch (StorageException<FileMetadata> ex)
        {
            throw
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}