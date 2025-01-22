using Microsoft.AspNetCore.Http;
using StorageService.Exceptions;
using StorageService.Model.Domain;
using StorageService.Repository.Interface;
using StorageService.Service.Interface;

namespace StorageService.Service;

public class FileManagementService : IFileManagementService
{
    private readonly IFileHandlerStrategy _fileHandlerStrategy;
    private readonly IFileRecordRepository _fileRecordRepository;
    private readonly ICheckSumService _checkSumService;
    
    public FileManagementService(
        IFileHandlerStrategy fileHandlerStrategy,
        IFileRecordRepository fileRecordRepository,
        ICheckSumService checkSumService
        )
    {
        _fileHandlerStrategy = fileHandlerStrategy;
        _fileRecordRepository = fileRecordRepository;
        _checkSumService = checkSumService;
    }
    
    public async Task<FileResultGeneric<FileMetadata>> UploadFileAsync(IFormFile file)
    {
        try
        {
            // Validate File type (second base) sos
            
            // Add a file record in database by extracting info from IFormFile with status Pending

            var calculatedChecksum = await _checkSumService.ComputeChecksumAsync(file);
            
            if(await _fileRecordRepository.CheckDuplicateFile(file, calculatedChecksum))
                return FileResultGeneric<FileMetadata>.Failure($"File {file.FileName} already exists.");
            
            var fileRecord = new FileRecord()
            {
                FileName = file.FileName,
                FileType = Path.GetExtension(file.FileName),
                FileSize = file.Length,
                UploadDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Status = FileStatus.Pending,
                Checksum = calculatedChecksum
            }.ToDto();
            
            await _fileRecordRepository.AddAsync(fileRecord);
            
            

            // Call storage service to save file at specific filepath

            // Change status of file record in db to Complete.


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
            throw;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}