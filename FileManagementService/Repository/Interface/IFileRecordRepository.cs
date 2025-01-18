using Microsoft.AspNetCore.Http;
using StorageService.Model.Domain;

namespace StorageService.Repository.Interface;

public interface IFileRecordRepository : IRepository<FileRecord>
{
    public Task<string> GetFilePathAsync(int id);
    
    public Task UpdateStatusAsync(int id, FileStatus status);
    
    public Task UpdateStatusAsync(string filePath, FileStatus status);
}