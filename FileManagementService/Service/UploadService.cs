using Microsoft.AspNetCore.Http;
using StorageService.Exceptions;
using StorageService.Model.Domain;
using StorageService.Repository.Interface;
using StorageService.Service.Interface;

namespace StorageService.Service;

public class UploadService : IUploadService
{
    private readonly ISaveFileStrategy _saveFileStrategy;
    private readonly IFileRecordRepository _fileRecordRepository;
    private readonly ICheckSumService _checkSumService;
    
    public UploadService(
        ISaveFileStrategy saveFileStrategy,
        IFileRecordRepository fileRecordRepository,
        ICheckSumService checkSumService
        )
    {
        _saveFileStrategy = saveFileStrategy;
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
            
            // Get storage strategy handler based on file type.
            var saveFileStrategyHandler = _saveFileStrategy.GetFileHandler(FileTypeMapper.GetFileTypeFromContentType(file.ContentType));
            
            var fileStorageResult = await SaveFileWithHandlingAsync(saveFileStrategyHandler, file, record.Id);
            
            // If result is corrupted, update db and return result
            if (!fileStorageResult.IsSuccess || fileStorageResult.Data is null)
            {
                await _fileRecordRepository.UpdateStatusAsync(record.Id, FileStatus.Failed);
                return fileStorageResult;
            }
            
            //Update file path and status of record
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
            throw new ApplicationException($"{typeof(UploadService)} Exception on Upload File Service {e.Message}, Stack Trace: {e.StackTrace}");
        }
    }
    
    /// <summary>
    /// Created separate function for this operation in order to encapsulate this
    /// into a try catch that calls the db in case of exception
    /// </summary>
    /// <param name="saveFileHandler"></param>
    /// <param name="file"></param>
    /// <param name="recordId"></param>
    /// <returns></returns>
    private async Task<FileResultGeneric<FileMetadata>> SaveFileWithHandlingAsync(ISaveFile saveFileHandler, IFormFile file, int recordId)
    {
        try
        {
            return await saveFileHandler.SaveFileAsync(file);
        }
        catch (Exception)
        {
            await _fileRecordRepository.UpdateStatusAsync(recordId, FileStatus.Failed);
            throw;
        }
    }
}