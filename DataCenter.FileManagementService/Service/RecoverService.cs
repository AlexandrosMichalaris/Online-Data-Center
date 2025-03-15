using AutoMapper;
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
    private readonly IFileRecordRepository _fileRecordRepository;
    private readonly IJobFileRecordRepository _jobFileRecordRepository;
    private readonly IRecoverFileService _recoverFileService;
    private readonly IMapper _mapper;

    #region Ctor

    public RecoverService(
        ILogger<RecoverService> logger, 
        IFileRecordRepository fileRecordRepository,
        IJobFileRecordRepository jobFileRecordRepository,
        IMapper mapper,
        IRecoverFileService recoverFileService
    )
    {
        _logger = logger;
        _fileRecordRepository = fileRecordRepository;
        _jobFileRecordRepository = jobFileRecordRepository;
        _recoverFileService = recoverFileService;
        _mapper = mapper;
    }

    #endregion
    
    
    public async Task<FileResultGeneric<FileMetadata>> RecoverFileAsync(int id)
    {
        try
        {
            var fileRecord = _mapper.Map<FileRecord>(await _fileRecordRepository.GetDeletedFileRecordAsync(id));
            
            if (fileRecord is null || !fileRecord.IsDeleted)
            {
                _logger.LogError($"{nameof(RecoverService)} - RecoverFileAsync failed. File Record {id} was not found.");
                return FileResultGeneric<FileMetadata>.Failure($"{nameof(RecoverService)} - RecoverFileAsync failed. File Record {id} was not found.");
            }
            
            // Get active job of fileRecord if exist
            var activeJob = await _jobFileRecordRepository.GetActiveJobOfFileRecordAsync(fileRecord.Id);

            if (activeJob is null)
            {
                _logger.LogError($"{nameof(RecoverService)} - RecoverFileAsync failed. No active job was found for record {id}.");
                return FileResultGeneric<FileMetadata>.Failure($"{nameof(RecoverService)} - RecoverFileAsync failed. No active job was found for record {id}.");
            }
            
            await _recoverFileService.RecoverFileAsync(fileRecord.FilePath);

            // Remove scheduled delete job
            BackgroundJob.Delete(activeJob.JobId.ToString());

            await _fileRecordRepository.RecoverAsync(fileRecord.Id);
            
            return FileResultGeneric<FileMetadata>.Success(new FileMetadata(
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