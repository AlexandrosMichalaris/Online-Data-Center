using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StorageService.Exceptions;
using StorageService.Model.Domain;
using StorageService.Repository.Interface;
using StorageService.Service.Interface;

namespace StorageService.Service;

public class UploadService : IUploadService
{
    private readonly ILogger<UploadService> _logger;
    private readonly ISaveFileStrategy _saveFileStrategy;
    private readonly IFileRecordRepository _fileRecordRepository;
    private readonly ICheckSumService _checkSumService;
    
    #region Ctor
    public UploadService(
        ISaveFileStrategy saveFileStrategy,
        IFileRecordRepository fileRecordRepository,
        ICheckSumService checkSumService,
        ILogger<UploadService> logger
    )
    {
        _logger = logger;
        _saveFileStrategy = saveFileStrategy;
        _fileRecordRepository = fileRecordRepository;
        _checkSumService = checkSumService;
    }
    #endregion

    public async Task<FileResultGeneric<FileMetadata>> UploadFileAsync(IFormFile file)
    {
        _logger.LogInformation($"{typeof(UploadService)} - UploadFileAsync - Uploading file {file.FileName}");
        
        try
        { //TODO: Validate File type (second base) sos
            //Calculate unique file hash
            var calculatedChecksum = await _checkSumService.ComputeChecksumAsync(file);
            
            if(await _fileRecordRepository.CheckDuplicateFile(file, calculatedChecksum))
                return FileResultGeneric<FileMetadata>.Failure($"File {file.FileName} already exists.", 400);
            
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
                _logger.LogError($"{typeof(UploadService)} - UploadFileAsync - File {file.FileName} could not be saved. FileStorageResult: {fileStorageResult}");
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
            _logger.LogError(ex, $"{typeof(UploadService)} - UploadFileAsync - Storage Exception on upload. {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{typeof(UploadService)} - UploadFileAsync failed. {ex.Message}");
            throw new ApplicationException($"{typeof(UploadService)} Exception on Upload File Service {ex.Message}, Stack Trace: {ex.StackTrace}");
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