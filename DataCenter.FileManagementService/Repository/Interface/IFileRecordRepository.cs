using FileProcessing.Model;
using Microsoft.AspNetCore.Http;
using StorageService.Model.Domain;

namespace StorageService.Repository.Interface;

public interface IFileRecordRepository : IRepository<FileRecordDto>
{
    Task UpdateStatusAsync(int id, FileStatus status);

    Task DeleteAsync(int id);

    Task RecoverAsync(int id);
    
    Task<FileRecordDto?> GetFileRecordByJobIdAsync(int jobId);
    
    Task<bool> CheckDuplicateFile(IFormFile file, string computedChecksum);
    
    Task<List<FileRecordDto>> GetScheduledDeletedFileRecordsAsync();
    
    Task<FileRecordDto?> GetDeletedFileRecordAsync(int id);
}