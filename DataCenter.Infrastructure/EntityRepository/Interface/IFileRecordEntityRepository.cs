using FileProcessing.Model;
using Microsoft.AspNetCore.Http;
using StorageService.Model.Domain;

namespace StorageService.Repository.Interface;

public interface IFileRecordEntityRepository : IEntityRepository<FileRecordEntity>
{
    Task<IEnumerable<FileRecordEntity>> GetPagedFileRecordAsync(int page, int pageSize);
    
    Task UpdateStatusAsync(Guid id, FileStatus status);

    Task DeleteAsync(Guid id);

    Task RecoverAsync(Guid id);
    
    Task<FileRecordEntity?> GetFileRecordByJobIdAsync(Guid jobId);
    
    Task<bool> CheckDuplicateFile(IFormFile file, string computedChecksum);
    
    Task<List<FileRecordEntity>> GetScheduledDeletedRecordsPagedAsync(int page, int pageSize);
    
    Task<FileRecordEntity?> GetDeletedFileRecordAsync(Guid id);
}