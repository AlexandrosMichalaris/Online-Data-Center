using FileProcessing.Model;
using Microsoft.AspNetCore.Http;
using StorageService.Model.Domain;

namespace StorageService.Repository.Interface;

public interface IFileRecordRepository : IRepository<FileRecordDto>
{
    public Task<string> GetFilePathAsync(int id);
    
    public Task UpdateStatusAsync(int id, FileStatus status);

    public Task DeleteAsync(int id);
    
    public Task<bool> CheckDuplicateFile(IFormFile file, string computedChecksum);
}