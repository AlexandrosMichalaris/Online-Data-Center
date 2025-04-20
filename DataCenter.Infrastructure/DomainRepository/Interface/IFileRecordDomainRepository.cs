using FileProcessing.Model;
using Microsoft.AspNetCore.Http;
using StorageService.Model.Domain;

namespace DataCenter.Infrastructure.Repository.DomainRepository.Interface;

public interface IFileRecordDomainRepository : IDomainRepository<FileRecord>
{
    Task<IEnumerable<FileRecord>> GetPagedFileRecordAsync(int page, int pageSize);
    
    Task UpdateStatusAsync(int id, FileStatus status);

    Task DeleteAsync(int id);

    Task RecoverAsync(int id);
    
    Task<FileRecord?> GetFileRecordByJobIdAsync(int jobId);
    
    Task<bool> CheckDuplicateFile(IFormFile file, string computedChecksum);
    
    Task<IEnumerable<FileRecord>> GetScheduledDeletedRecordsPagedAsync(int page, int pageSize);
    
    Task<FileRecord?> GetDeletedFileRecordAsync(int id);
}