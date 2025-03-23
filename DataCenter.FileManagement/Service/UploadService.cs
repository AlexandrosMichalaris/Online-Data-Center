using AutoMapper;
using DataCenter.Infrastructure.Repository.DomainRepository.Interface;
using FileProcessing.Model;
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
    private readonly IFileRecordDomainRepository _fileRecordDomainRepository;
    private readonly ICheckSumService _checkSumService;
    
    #region Ctor
    public UploadService(
        ISaveFileStrategy saveFileStrategy,
        IFileRecordDomainRepository fileRecordDomainRepository,
        ICheckSumService checkSumService,
        ILogger<UploadService> logger
    )
    {
        _logger = logger;
        _saveFileStrategy = saveFileStrategy;
        _fileRecordDomainRepository = fileRecordDomainRepository;
        _checkSumService = checkSumService;
    }
    #endregion

    public async Task<FileResultGeneric<FileMetadata>> UploadFileAsync(IFormFile file, string connectionId)
    {
        _logger.LogInformation($"{nameof(UploadService)} - UploadFileAsync - Uploading file {file.FileName}");
        
        try
        { //TODO: Validate File type (second base)
            //Calculate unique file hash
            var calculatedChecksum = await _checkSumService.ComputeChecksumAsync(file);
            
            if(await _fileRecordDomainRepository.CheckDuplicateFile(file, calculatedChecksum))
                return FileResultGeneric<FileMetadata>.Failure($"File {file.FileName} already exists.", 400);
            
            //Build file record dto object (because of filepath)
            var fileRecord = new FileRecord(
                fileName: file.FileName,
                fileType: FileTypeMapper.GetFileTypeFromContentType(file.ContentType).ToString(),
                status: FileStatus.Pending, checksum: calculatedChecksum, fileSize: file.Length);
            
            // Get storage strategy handler based on file type.
            var saveFileStrategyHandler = _saveFileStrategy.GetFileHandler(FileTypeMapper.GetFileTypeFromContentType(file.ContentType));
            
            var record = await _fileRecordDomainRepository.AddAsync(fileRecord);
            
            var fileStorageResult = await SaveFileWithHandlingAsync(saveFileStrategyHandler, file, record.Id, connectionId);
            
            // If result is corrupted, update db and return result
            if (!fileStorageResult.IsSuccess || fileStorageResult.Data is null)
            {
                _logger.LogError($"{nameof(UploadService)} - UploadFileAsync - File {file.FileName} could not be saved. FileStorageResult: {fileStorageResult}");
                await _fileRecordDomainRepository.UpdateStatusAsync(record.Id, FileStatus.Failed);
                return fileStorageResult;
            }
            
            //Update file path and status of record
            record.FilePath = fileStorageResult.Data.FilePath;
            record.Status = FileStatus.Completed;
            
            await _fileRecordDomainRepository.UpdateAsync(record);

            return fileStorageResult;
        }
        catch (StorageException<FileMetadata> ex)
        {
            _logger.LogError(ex, $"{nameof(UploadService)} - UploadFileAsync - Storage Exception on upload. {ex.Message}");
            throw;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, $"{nameof(UploadService)} - UploadFileAsync - Invalid Operation Exception on upload. {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(UploadService)} - UploadFileAsync failed. {ex.Message}, Stack Trace: {ex.StackTrace}");
            throw new ApplicationException($"{nameof(UploadService)} Exception on Upload File Service {ex.Message}");
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
    private async Task<FileResultGeneric<FileMetadata>> SaveFileWithHandlingAsync(ISaveFile saveFileHandler, IFormFile file, int recordId, string connectionId)
    {
        try
        {
            return await saveFileHandler.SaveFileAsync(file, connectionId);
        }
        catch (Exception)
        {
            await _fileRecordDomainRepository.UpdateStatusAsync(recordId, FileStatus.Failed);
            throw;
        }
    }
}