using Data_Center.Configuration.Constants;
using Microsoft.Extensions.Logging;
using StorageService.Exceptions;
using StorageService.Model;
using StorageService.Repository.Interface;
using StorageService.Service.Interface;

namespace StorageService.Service;

public class DownloadService : IDownloadService
{
    private readonly ILogger<DownloadService> _logger;
    private readonly IFileRecordRepository _fileRecordRepository;
    private readonly IGetFileStreamService _getFileStreamService;

    #region Ctor
    public DownloadService(
        ILogger<DownloadService> logger,
        IFileRecordRepository fileRecordRepository,
        IGetFileStreamService getFileStreamService
    )
    {
        _logger = logger;
        _fileRecordRepository = fileRecordRepository;
        _getFileStreamService = getFileStreamService;
    }
    #endregion

    public async Task<FileResultGeneric<StreamData>> DownloadFileAsync(int id)
    {
        try
        {
            var fileRecord = (await _fileRecordRepository.GetByIdAsync(id))?.ToDomain();
            if (fileRecord is null)
            {
                _logger.LogError($"{nameof(DownloadService)} - DownloadFileAsync failed. File Record {id} was not found.");
                throw new FileNotFoundException($"{nameof(DownloadService)} - DownloadFileAsync failed. File Record {id} was not found.");
            }
            
            var stream = await _getFileStreamService.GetFileStreamAsync(fileRecord.FilePath);
            if (stream.Data is null || !stream.IsSuccess)
            {
                _logger.LogError($"{nameof(DownloadService)} - DownloadFileAsync failed. Stream Data failed.");
                throw new FileNotFoundException($"{nameof(DownloadService)} - DownloadFileAsync failed. File Record {id} was not found.");
            }
            
            return FileResultGeneric<StreamData>.Success(new StreamData()
            {
                FileName = fileRecord.FileName,
                FileContentType = FileTypeMapper.GetContentTypeFromFileType(Enum.Parse<FileType>(fileRecord.FileType)),
                Stream = stream.Data
            });
        }
        catch (StorageException<Stream> ex)
        {
            _logger.LogError(ex, $"{nameof(DownloadService)} - DownloadFileAsync - Storage Exception on download. {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(DownloadService)} - DownloadFileAsync failed. {ex.Message}");
            throw new ApplicationException($"{nameof(DownloadService)} Exception on Download File Service {ex.Message}, Stack Trace: {ex.StackTrace}");
        }
    }
    
    public async Task<FileResultGeneric<StreamData>> DownloadFileAsync(string filePath)
    {
        try
        {
            throw new System.NotImplementedException();
        }
        catch (StorageException<Stream> ex)
        {
            _logger.LogError(ex, $"{nameof(DownloadService)} - DownloadFileAsync - Storage Exception on download. {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(DownloadService)} - DownloadFileAsync failed. {ex.Message}");
            throw new ApplicationException($"{nameof(DownloadService)} Exception on Download File Service {ex.Message}, Stack Trace: {ex.StackTrace}");
        }
    }

    public Task<FileResultGeneric<Stream>> PreviewFileAsync(string filePath)
    {
        try
        {
            //Only for PDF Files
            throw new System.NotImplementedException();
        }
        catch (StorageException<Stream> ex)
        {
            _logger.LogError(ex, $"{nameof(DownloadService)} - PreviewFileAsync - Storage Exception on preview. {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(DownloadService)} - PreviewFileAsync failed. {ex.Message}");
            throw new ApplicationException($"{nameof(DownloadService)} Exception on Preview File Service {ex.Message}, Stack Trace: {ex.StackTrace}");
        }
    }
}