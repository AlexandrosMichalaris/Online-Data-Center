using Data_Center.Configuration.Database;
using FileProcessing.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StorageService.Model.Domain;
using StorageService.Repository.Interface;

namespace DataCenter.Infrastructure.Repository.EntityRepository;

/// <summary>
/// File Record Repository
/// Every operation done here, is considered to be done into the storage too.
/// </summary>
public class FileRecordEntityRepository : EntityRepository<FileRecordEntity, DatabaseContext>, IFileRecordEntityRepository
{
    private readonly DatabaseContext _dbContext;
    private readonly DbSet<FileRecordEntity> _dbSet;
    private readonly ILogger<FileRecordEntityRepository> _logger;

    #region Ctor

    public FileRecordEntityRepository(DatabaseContext dbContext, ILogger<FileRecordEntityRepository> logger) : base(dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<FileRecordEntity>();
        _logger = logger;
    }

    #endregion
    
    public async Task UpdateStatusAsync(int id, FileStatus status)
    {
        var record = await _dbSet.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);

        if (record is not null)
        {
            // Re-attach a new instance and update
            record.Status = status;
            _dbContext.Entry(record).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();
        }
    }
    
    public override async Task DeleteAsync(int id)
    {
        var entity = await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);

        if (entity is not null)
        {
            entity.IsDeleted = true;
            _dbContext.Entry(entity).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task RecoverAsync(int id)
    {
        var entity = await _dbSet
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.Status == FileStatus.Completed && x.IsDeleted)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (entity is not null)
        {
            entity.IsDeleted = false;
            _dbContext.Entry(entity).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();
        }
    }
    
    public async Task<FileRecordEntity?> GetFileRecordByJobIdAsync(int jobId)
    {
        var job = await _dbContext.JobFileRecords.AsNoTracking().FirstOrDefaultAsync(j => j.Id == jobId);

        if (job is null)
        {
            _logger.LogError(
                $"{nameof(FileRecordEntityRepository)} - GetFileRecordByJobIdAsync - Job id not found in Database: {jobId}.");
            throw new InvalidOperationException(
                $"{nameof(FileRecordEntityRepository)} - GetFileRecordByJobIdAsync - Job id not found in Database: {jobId}.");
        }

        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(f => f.Id == job.FileId);
    }
    
    public async Task<List<FileRecordEntity>> GetScheduledDeletedFileRecordsAsync()
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.Status == FileStatus.Completed && x.IsDeleted) 
            .Where(file => _dbContext.JobFileRecords.Any(jfr => jfr.FileId == file.Id)) //Check if there is JobFileRecord 
            .ToListAsync();
    }

    public async Task<FileRecordEntity?> GetDeletedFileRecordAsync(int id)
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .AsNoTracking()
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
        return await _dbSet.AsNoTracking().AnyAsync(f => 
                f.FileName == file.FileName ||
                f.Checksum == computedChecksum
            //&& f.IsDeleted == false
        );
    }
}