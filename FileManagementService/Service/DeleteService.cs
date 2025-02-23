using Hangfire;
using Microsoft.Extensions.Logging;
using StorageService.Exceptions;
using StorageService.Repository.Interface;
using StorageService.Service.Interface;

namespace StorageService.Service;

public class DeleteService : IDeleteService
{
    private readonly ILogger<DeleteService> _logger;
    private readonly IFileRecordRepository _fileRecordRepository;
    private readonly IDeleteFileService _deleteFileService;

    #region MyRegion

    public DeleteService(
        ILogger<DeleteService> logger, 
        IFileRecordRepository fileRecordRepository,
        IDeleteFileService deleteFileService)
    {
        _logger = logger;
        _fileRecordRepository = fileRecordRepository;
        _deleteFileService = deleteFileService;
    }

    #endregion
    
    public async Task<FileResultGeneric<FileMetadata>> DeleteFileAsync(int id)
    {
        try
        {
            var fileRecord = (await _fileRecordRepository.GetByIdAsync(id))?.ToDomain();
            if (fileRecord is null)
            {
                _logger.LogError($"{nameof(DeleteService)} - DeleteFileAsync failed. File Record {id} was not found.");
                return FileResultGeneric<FileMetadata>.Failure($"{nameof(DeleteService)} - DeleteFileAsync failed. File Record {id} was not found.");
            }
            
            BackgroundJob.Schedule(() => _deleteFileService.DeleteFile(fileRecord.FilePath), TimeSpan.FromDays(30));

            await _fileRecordRepository.DeleteAsync(id);
            
            return FileResultGeneric<FileMetadata>.Success(new FileMetadata(
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
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(DeleteService)} - DeleteFileAsync failed. {ex.Message}, Stack Trace: {ex.StackTrace}");
            throw new ApplicationException($"{nameof(DeleteService)} Exception on Delete File Service {ex.Message}");
        }
    }

    public Task<FileResultGeneric<string>> RecoverFileAsync(int id)
    {
        throw new NotImplementedException();
    }
}