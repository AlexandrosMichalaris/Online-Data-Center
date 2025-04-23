using FileProcessing.Model;
using Microsoft.AspNetCore.Http;
using StorageService.Model.Domain;

namespace DataCenter.Infrastructure.Repository.DomainRepository.Interface;

public interface IFileRecordDomainRepository : IDomainRepository<FileRecord>
{
    Task<IEnumerable<FileRecord>> GetPagedFileRecordAsync(int page, int pageSize);
    
    Task UpdateStatusAsync(Guid id, FileStatus status);

    Task DeleteAsync(Guid id);

    Task RecoverAsync(Guid id);
    
    Task<FileRecord?> GetFileRecordByJobIdAsync(Guid jobId);
    
    Task<bool> CheckDuplicateFile(IFormFile file, string computedChecksum);
    
    Task<IEnumerable<FileRecord>> GetScheduledDeletedRecordsPagedAsync(int page, int pageSize);
    
    Task<FileRecord?> GetDeletedFileRecordAsync(Guid id);
}