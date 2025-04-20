using AutoMapper;
using DataCenter.Domain.Dto;
using DataCenter.Infrastructure.Repository.DomainRepository.Interface;
using Microsoft.Extensions.Logging;
using StorageService.Service.Interface;

namespace StorageService.Service;

public class FileInfoService : IFileInfoService
{
    private readonly ILogger<FileInfoService> _logger;
    private readonly IFileRecordDomainRepository _fileRecordDomainRepository;
    private readonly IMapper _mapper;

    public FileInfoService(
        IFileRecordDomainRepository fileRecordDomainRepository, 
        IMapper mapper,
        ILogger<FileInfoService> logger)
    {
        _fileRecordDomainRepository = fileRecordDomainRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<FileResultGeneric<IEnumerable<FileRecordMetadata>>> GetPagedFileRecordsAsync(int page, int pageSize, bool isDeleted = false)
    {
        _logger.LogInformation($"Fetching paged file records. Page: {page}, PageSize: {pageSize}");

        if (page <= 0 || pageSize <= 0)
        {
            const string errorMsg = "Page and PageSize must be greater than 0.";
            _logger.LogWarning($"Invalid pagination parameters. {errorMsg}");
            return FileResultGeneric<IEnumerable<FileRecordMetadata>>.Failure(errorMsg);
        }

        try
        {
            var records = isDeleted ?
                await _fileRecordDomainRepository.GetScheduledDeletedRecordsPagedAsync(page, pageSize)
                : await _fileRecordDomainRepository.GetPagedFileRecordAsync(page, pageSize);
            
            if (!records.Any())
            {
                var message = $"No file records found for page {page} with page size {pageSize}.";
                _logger.LogInformation(message);
                return FileResultGeneric<IEnumerable<FileRecordMetadata>>.Success(Enumerable.Empty<FileRecordMetadata>());
            }

            var fileMetadataList = records.Select(f => new FileRecordMetadata {
                Id = f.Id,
                FileName = f.FileName,
                ContentType = f.FileType,
                Size = f.FileSize,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt,
                IsDeleted = f.IsDeleted,
            });

            _logger.LogInformation($"Fetched {fileMetadataList.Count()} file records for page {page}");

            return FileResultGeneric<IEnumerable<FileRecordMetadata>>.Success(fileMetadataList);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, $"{nameof(FileInfoService)} - GetPagedFileRecordsAsync - Invalid Operation Exception on download. {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while fetching paged file records for page {page} with pageSize {pageSize}.");
            throw new ApplicationException("An unexpected error occurred while retrieving file records.");
        }
    }
    
}