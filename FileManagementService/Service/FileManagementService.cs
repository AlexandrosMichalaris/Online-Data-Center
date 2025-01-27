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
        { //TODO: Validate File type (second base) sos
            var calculatedChecksum = await _checkSumService.ComputeChecksumAsync(file);
            
            if(await _fileRecordRepository.CheckDuplicateFile(file, calculatedChecksum))
                return FileResultGeneric<FileMetadata>.Failure($"File {file.FileName} already exists.");
            
            var fileRecord = new FileRecord()
            {
                FileName = file.FileName,
                FileType = FileTypeMapper.GetFileTypeFromContentType(file.ContentType).ToString(),
                Status = FileStatus.Pending,
                Checksum = calculatedChecksum
            }.ToDto();
            
            var record = await _fileRecordRepository.AddAsync(fileRecord);
            
            var fileMetadata = await _fileHandlerStrategy
                .GetFileHandler(FileTypeMapper.GetFileTypeFromContentType(file.ContentType))
                .SaveFileAsync(file);
            
            await _fileRecordRepository.UpdateStatusAsync(record.Id, FileStatus.Completed);

            return fileMetadata;
        }
        catch (StorageException<FileMetadata> ex)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new ApplicationException($"{typeof(FileManagementService)} Exception on Upload File Service {e.Message}, Stack Trace: {e.StackTrace}");
        }
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
            throw new ApplicationException($"{typeof(FileManagementService)} Exception on Download File Service {e.Message}, Stack Trace: {e.StackTrace}");
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
            throw new ApplicationException($"{typeof(FileManagementService)} Exception on Preview File Service {e.Message}, Stack Trace: {e.StackTrace}");
        }
    }
}