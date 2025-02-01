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
            //Calculate unique file hash
            var calculatedChecksum = await _checkSumService.ComputeChecksumAsync(file);
            
            if(await _fileRecordRepository.CheckDuplicateFile(file, calculatedChecksum))
                return FileResultGeneric<FileMetadata>.Failure($"File {file.FileName} already exists.");
            
            //Build file record dto object
            var fileRecord = new FileRecord()
            {
                FileName = file.FileName,
                FileType = FileTypeMapper.GetFileTypeFromContentType(file.ContentType).ToString(),
                Status = FileStatus.Pending,
                Checksum = calculatedChecksum,
                FileSize = file.Length
            }.ToDto();
            
            var record = await _fileRecordRepository.AddAsync(fileRecord);

            FileResultGeneric<FileMetadata> fileStorageResult;
            
            // If an exception occurs on storage service, change status to failed.
            try
            {
                fileStorageResult = await _fileHandlerStrategy
                    .GetFileHandler(FileTypeMapper.GetFileTypeFromContentType(file.ContentType))
                    .SaveFileAsync(file);
            }
            catch (Exception e)
            {
                await _fileRecordRepository.UpdateStatusAsync(record.Id, FileStatus.Failed);
                throw;
            }
            
            //Update file path and status of record
            if (!fileStorageResult.IsSuccess)
            {
                await _fileRecordRepository.UpdateStatusAsync(record.Id, FileStatus.Failed);
                return fileStorageResult;
            }
            
            record.FilePath = fileStorageResult.Data.FilePath;
            record.Status = (int)FileStatus.Completed;
            
            await _fileRecordRepository.UpdateAsync(record);

            return fileStorageResult;
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