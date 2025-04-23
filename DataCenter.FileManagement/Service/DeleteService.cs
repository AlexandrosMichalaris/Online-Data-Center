using AutoMapper;
using DataCenter.Infrastructure.Repository.DomainRepository.Interface;
using Hangfire;
using Microsoft.Extensions.Logging;
using StorageService.Exceptions;
using StorageService.Model.Domain;
using StorageService.Service.Interface;

namespace StorageService.Service;

public class DeleteService : IDeleteService
{
    private readonly ILogger<DeleteService> _logger;
    private readonly IFileRecordDomainRepository _fileRecordDomainRepository;
    private readonly IDeleteFileService _deleteFileService;
    private readonly IJobFileRecordDomainRepository _jobFileRecordDomainRepository;
    private readonly IMapper _mapper;

    #region Ctor

    public DeleteService(
        ILogger<DeleteService> logger, 
        IFileRecordDomainRepository fileRecordDomainRepository,
        IDeleteFileService deleteFileService,
        IJobFileRecordDomainRepository jobFileRecordDomainRepository,
        IMapper mapper)
    {
        _logger = logger;
        _fileRecordDomainRepository = fileRecordDomainRepository;
        _deleteFileService = deleteFileService;
        _jobFileRecordDomainRepository = jobFileRecordDomainRepository;
        _mapper = mapper;
    }

    #endregion
    
    public async Task<FileResultGeneric<FileMetadata>> DeleteFileAsync(Guid id)
    {
        try
        {   
            var fileRecord = await _fileRecordDomainRepository.GetByIdAsync(id);
            if (fileRecord is null)
            {
                _logger.LogError($"{nameof(DeleteService)} - DeleteFileAsync failed. File Record {id} was not found.");
                return FileResultGeneric<FileMetadata>.Failure($"{nameof(DeleteService)} - DeleteFileAsync failed. File Record {id} was not found.");
            }

            var (fileName, trashFolder) = StorageHelper.GetFileNameAndTrashFolder(fileRecord.FilePath);
            var fileExistsInTrash = StorageHelper.FileExists(Path.Combine(trashFolder, fileName));

            await _deleteFileService.RecycleFileAsync(fileRecord.FilePath);

            var action = fileExistsInTrash 
                ? HandleFileInTrashAsync(fileRecord) 
                : ScheduleFileDeletionAsync(fileRecord);
            await action;
            
            // SoftDelete on Db
            await _fileRecordDomainRepository.DeleteAsync(id);
            
            return FileResultGeneric<FileMetadata>.Success(new FileMetadata(
                fileId: fileRecord.Id,
                filePath: fileRecord.FilePath,
                fileName: fileRecord.FileName,
                fileSize: fileRecord.FileSize,
                mimeType: fileRecord.FileType,
                uploadTime: DateTime.UtcNow,
                storageFolder: null
            ));
        }
        catch (StorageException<Stream> ex)
        {
            _logger.LogError(ex, $"{nameof(DeleteService)} - DeleteFileAsync - Storage Exception on delete. {ex.Message}");
            throw;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, $"{nameof(DeleteService)} - DeleteFileAsync - Invalid Operation Exception on download. {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(DeleteService)} - DeleteFileAsync failed. {ex.Message}, Stack Trace: {ex.StackTrace}");
            throw new ApplicationException($"{nameof(DeleteService)} Exception on Delete File Service {ex.Message}");
        }
    }
    
    private async Task HandleFileInTrashAsync(FileRecord fileRecord)
    {
        var activeJob =
            await _jobFileRecordDomainRepository.GetActiveJobOfFileRecordAsync(fileRecord.FileName, fileRecord.Checksum);

        if (activeJob is null)
        {
            var errorMessage = $"{nameof(DeleteService)} - DeleteFileAsync - FileRecord {fileRecord.Id} has no active job.";
            _logger.LogError(errorMessage);
            throw new ApplicationException(errorMessage);
        }
        
        BackgroundJob.Reschedule(activeJob.JobId.ToString(), TimeSpan.FromDays(30));
    }

    private async Task ScheduleFileDeletionAsync(FileRecord fileRecord)
    {
        var jobId = BackgroundJob.Schedule(() => DeleteJobFiles(fileRecord), TimeSpan.FromDays(30));
        var jobRecord = new JobFileRecord
        {
            FileId = fileRecord.Id,
            JobId = long.Parse(jobId),
            FileName = fileRecord.FileName,
            Checksum = fileRecord.Checksum,
        };

        await _jobFileRecordDomainRepository.AddAsync(jobRecord);
    }
    
    /// <summary>
    /// Function is public because job scheduling needs a public function
    /// </summary>
    /// <param name="fileRecord"></param>
    public void DeleteJobFiles(FileRecord fileRecord)
    {
        _deleteFileService.DeleteFileFromTrashAsync(fileRecord.FilePath).GetAwaiter().GetResult();
        _jobFileRecordDomainRepository.DeleteJobByRecordIdAsync(fileRecord.Id).GetAwaiter().GetResult();
    }
}