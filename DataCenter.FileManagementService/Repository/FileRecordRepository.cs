using Data_Center.Configuration.Database;
using FileProcessing.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StorageService.Model.Domain;
using StorageService.Repository.Interface;

namespace StorageService.Repository;

/// <summary>
/// File Record Repository
/// Every operation done here, is considered to be done into the storage too.
/// </summary>
public class FileRecordRepository : Repository<FileRecordEntity>, IFileRecordRepository
{
    private readonly DatabaseContext _dbContext;
    private readonly DbSet<FileRecordEntity> _dbSet;
    private readonly ILogger<FileRecordRepository> _logger;

    #region Ctor

    public FileRecordRepository(DatabaseContext dbContext, ILogger<FileRecordRepository> logger) : base(dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<FileRecordEntity>();
        _logger = logger;
    }

    #endregion
    
    public async Task UpdateStatusAsync(int id, FileStatus status)
    {
        var record = await _dbSet.FindAsync(id);

        if (record is not null)
        {
            record.Status = status;
            await _dbContext.SaveChangesAsync();
        }
    }
    
    public override async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity is not null)
        {
            entity.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task RecoverAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity is not null)
        {
            entity.IsDeleted = false;
            await _dbContext.SaveChangesAsync();
        }
    }
    
    public async Task<FileRecordEntity?> GetFileRecordByJobIdAsync(int jobId)
    {
        var job = await _dbContext.JobFileRecords.FindAsync(jobId);

        if (job is null)
        {
            _logger.LogError(
                $"{nameof(FileRecordRepository)} - GetFileRecordByJobIdAsync - Job id not found in Database: {jobId}.");
            throw new InvalidOperationException(
                $"{nameof(FileRecordRepository)} - GetFileRecordByJobIdAsync - Job id not found in Database: {jobId}.");
        }
        
        return await _dbSet.FindAsync(job.FileId);
    }
    
    public async Task<List<FileRecordEntity>> GetScheduledDeletedFileRecordsAsync()
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .Where(x => x.Status == FileStatus.Completed && x.IsDeleted) 
            .Where(file => _dbContext.JobFileRecords.Any(jfr => jfr.FileId == file.Id)) //Check if there is JobFileRecord 
            .ToListAsync();
    }

    public async Task<FileRecordEntity?> GetDeletedFileRecordAsync(int id)
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .Where(x => x.Status == FileStatus.Completed && x.IsDeleted) 
            .FirstOrDefaultAsync(file => file.Id == id);
    }

    /// <summary>
    /// Data Storage will not accept duplicate file name or same bytes of file.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="computedChecksum"></param>
    /// <returns></returns>
    public async Task<bool> CheckDuplicateFile(IFormFile file, string computedChecksum)
    {
        // Check against database
        return await _dbSet.AnyAsync(f => 
                f.FileName == file.FileName ||
                f.Checksum == computedChecksum
            //&& f.IsDeleted == false
        );
    }
}