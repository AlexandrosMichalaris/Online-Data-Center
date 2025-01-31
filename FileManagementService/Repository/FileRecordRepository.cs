using Data_Center.Configuration.Database;
using FileProcessing.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StorageService.Model.Domain;
using StorageService.Repository.Interface;

namespace StorageService.Repository;

/// <summary>
/// File Record Repository
/// Every operation done here, is considered to be done into the storage too.
/// </summary>
public class FileRecordRepository : Repository<FileRecordDto>, IFileRecordRepository
{
    private readonly DatabaseContext _dbContext;
    private readonly DbSet<FileRecordDto> _dbSet;
    
    public FileRecordRepository(DatabaseContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<FileRecordDto>();
    }
    

    public async Task<string> GetFilePathAsync(int id)
    {
        try
        {
            var record = await _dbSet.FindAsync(id);

            if (record is null)
                throw new ApplicationException($"Record with id {id} not found");
            
            return await Task.FromResult(record.FilePath);
        }
        catch (Exception e)
        {
            throw new ApplicationException($"{typeof(FileRecordRepository)} Could not GET FILE PATH of record in Database: {e.Message}");
        }
    }

    public async Task UpdateStatusAsync(int id, FileStatus status)
    {
        try
        {
            var record = await _dbSet.FindAsync(id);

            if (record is not null)
            {
                record.Status = (int)status;
                await _dbContext.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            throw new ApplicationException($"{typeof(FileRecordRepository)} Status of Record with id {id} could not be updated: {e.Message}");
        }
    }


    public override async Task DeleteAsync(int id)
    {
        try
        {
            var entity = await GetByIdAsync(id);
            if (entity is not null)
            {
                entity.IsDeleted = true;
                await _dbContext.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            throw new ApplicationException($"{typeof(Repository<FileRecordDto>)} Could not REMOVE record in Database: {e.Message}");
        }
    }

    public async Task<bool> CheckDuplicateFile(IFormFile file, string computedChecksum)
    {
        // Check against database
        return await _dbSet.AnyAsync(f => 
            f.Checksum == computedChecksum &&
            f.IsDeleted == false
        );
    }
}