using FileProcessing.Model;
using Microsoft.AspNetCore.Http;
using StorageService.Model.Domain;

namespace StorageService.Repository.Interface;

public interface IFileRecordEntityRepository : IEntityRepository<FileRecordEntity>
{
    Task UpdateStatusAsync(int id, FileStatus status);

    Task DeleteAsync(int id);

    Task RecoverAsync(int id);
    
    Task<FileRecordEntity?> GetFileRecordByJobIdAsync(int jobId);
    
    Task<bool> CheckDuplicateFile(IFormFile file, string computedChecksum);
    
    Task<List<FileRecordEntity>> GetScheduledDeletedFileRecordsAsync();
    
    Task<FileRecordEntity?> GetDeletedFileRecordAsync(int id);
}