using DataCenter.Domain.Dto;

namespace StorageService.Service.Interface;

public interface IFileInfoService
{
    Task<FileResultGeneric<IEnumerable<FileRecordMetadata>>> GetPagedFileRecordsAsync(int page, int pageSize);
}