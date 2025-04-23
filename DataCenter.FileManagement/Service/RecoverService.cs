using AutoMapper;
using DataCenter.Infrastructure.Repository.DomainRepository.Interface;
using Hangfire;
using Microsoft.Extensions.Logging;
using StorageService.Exceptions;
using StorageService.Model.Domain;
using StorageService.Repository.Interface;
using StorageService.Service.Interface;

namespace StorageService.Service;

public class RecoverService : IRecoverService
{
    private readonly ILogger<RecoverService> _logger;
    private readonly IFileRecordDomainRepository _fileRecordDomainRepository;
    private readonly IJobFileRecordDomainRepository _jobFileRecordDomainRepository;
    private readonly IRecoverFileService _recoverFileService;

    #region Ctor

    public RecoverService(
        ILogger<RecoverService> logger, 
        IFileRecordDomainRepository fileRecordDomainRepository,
        IJobFileRecordDomainRepository jobFileRecordDomainRepository,
        IRecoverFileService recoverFileService
    )
    {
        _logger = logger;
        _fileRecordDomainRepository = fileRecordDomainRepository;
        _jobFileRecordDomainRepository = jobFileRecordDomainRepository;
        _recoverFileService = recoverFileService;
    }

    #endregion
    
    public async Task<FileResultGeneric<FileMetadata>> RecoverFileAsync(Guid id)
    {
        try
        {
            var fileRecord = await _fileRecordDomainRepository.GetDeletedFileRecordAsync(id);
            
            if (fileRecord is null || !fileRecord.IsDeleted)
            {
                _logger.LogError($"{nameof(RecoverService)} - RecoverFileAsync failed. File Record {id} was not found.");
                return FileResultGeneric<FileMetadata>.Failure($"{nameof(RecoverService)} - RecoverFileAsync failed. File Record {id} was not found.");
            }
            
            // Get active job of fileRecord if exist
            var activeJob = await _jobFileRecordDomainRepository.GetActiveJobOfFileRecordAsync(fileRecord.Id);

            if (activeJob is null)
            {
                _logger.LogError($"{nameof(RecoverService)} - RecoverFileAsync failed. No active job was found for record {id}.");
                return FileResultGeneric<FileMetadata>.Failure($"{nameof(RecoverService)} - RecoverFileAsync failed. No active job was found for record {id}.");
            }
            
            // If file already exists in original folder, this means there is an existing fileRecord
            // with isDelete false. So there is no need to recover the deleted file record entry.
            if(!StorageHelper.FileExists(fileRecord.FilePath))
                await _fileRecordDomainRepository.RecoverAsync(fileRecord.Id); // Make isDelete property to false
            
            // Move file from trash to original folder. If it exists, it overrides it.
            await _recoverFileService.RecoverFileAsync(fileRecord.FilePath);

            // Remove scheduled delete job
            BackgroundJob.Delete(activeJob.JobId.ToString());
            
            // Delete job record entry
            await _jobFileRecordDomainRepository.DeleteJobByRecordIdAsync(fileRecord.Id);
            
            return FileResultGeneric<FileMetadata>.Success(new FileMetadata(
                fileId: fileRecord.Id,
                filePath: fileRecord.FilePath,
                fileName: fileRecord.FileName,
                fileSize: fileRecord.FileSize,
                mimeType: fileRecord.FileType,
                uploadTime: DateTime.UtcNow,
                storageFolder: StorageHelper.GetDirectoryName(fileRecord.FilePath)
            ));
        }
        catch (StorageException<Stream> ex)
        {
            _logger.LogError(ex, $"{nameof(RecoverService)} - RecoverFileAsync - Storage Exception on recover. {ex.Message}");
            throw;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, $"{nameof(RecoverService)} - RecoverFileAsync - Invalid Operation Exception on download. {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(RecoverService)} - RecoverFileAsync failed. {ex.Message}, Stack Trace: {ex.StackTrace}");
            throw new ApplicationException($"{nameof(RecoverService)} Exception on Recover File Service {ex.Message}");
        }
    }
}