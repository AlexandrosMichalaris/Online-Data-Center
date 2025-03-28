using System.Linq.Expressions;
using Data_Center.Configuration.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StorageService.Model.Entities;
using StorageService.Repository.Interface;

namespace DataCenter.Infrastructure.Repository.EntityRepository;

public class JobFileRecordEntityRepository : EntityRepository<JobFileRecordEntity>, IJobFileRecordEntityRepository
{
    private readonly DatabaseContext _dbContext;
    private readonly DbSet<JobFileRecordEntity> _dbSet;
    private readonly ILogger<JobFileRecordEntityRepository> _logger;

    #region Ctor

    public JobFileRecordEntityRepository(DatabaseContext dbContext, ILogger<JobFileRecordEntityRepository> logger) : base(dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<JobFileRecordEntity>();
        _logger = logger;
    }

    #endregion

    public async Task<IEnumerable<JobFileRecordEntity>> GetFileRecordJobsAsync(int fileRecordId)
    {
        return await _dbSet
            .Where(job => job.FileId == fileRecordId)
            .ToListAsync();
    }

    public async Task<JobFileRecordEntity?> GetActiveJobOfFileRecordAsync(string fileName, string checksum)
    {
        return await GetSingleActiveJobAsync(job => job.FileName == fileName && job.Checksum == checksum, 
            $"FileRecord Name: {fileName}");
    }

    public async Task<JobFileRecordEntity?> GetActiveJobOfFileRecordAsync(int fileRecordId)
    {
        return await GetSingleActiveJobAsync(job => job.FileId == fileRecordId, 
            $"FileRecordId: {fileRecordId}");
    }

    public async Task DeleteJobByRecordIdAsync(int recordId)
    {
        var job = await _dbSet
            .FirstOrDefaultAsync(job => job.FileId == recordId);

        if (job is null)
        {
            _logger.LogWarning("{Repo} - No job found to delete for FileRecordId: {FileRecordId}.",
                nameof(JobFileRecordEntityRepository), recordId);
            return;
        }

        _dbSet.Remove(job);

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("{Repo} - Deleted job with JobId: {JobId} for FileRecordId: {FileRecordId}.",
            nameof(JobFileRecordEntityRepository), job.Id, recordId);
    }
    
    public List<JobFileRecordEntity> GetPendingJobs()
    {
        return _dbContext.JobFileRecords
            .Join(
                _dbContext.HangfireJobs,
                jfr => jfr.JobId,
                hjob => hjob.Id,
                (jfr, hjob) => new { jfr, hjob }
            )
            .GroupJoin(
                _dbContext.HangfireStates,
                joinResult => joinResult.hjob.Id,
                state => state.JobId,
                (joinResult, states) => new
                {
                    joinResult.jfr,
                    latestState = states
                        .OrderByDescending(s => s.CreatedAt) // assuming CreatedAt is the field name
                        .FirstOrDefault()
                }
            )
            .Where(result => result.latestState != null &&
                             (result.latestState.Name == "Scheduled" || result.latestState.Name == "Enqueued"))
            .Select(result => new JobFileRecordEntity
            {
                Id = result.jfr.Id,
                FileId = result.jfr.FileId,
                JobId = result.jfr.JobId,
                FileName = result.jfr.FileName,
                ScheduledAt = result.jfr.ScheduledAt
            })
            .ToList();
    }
    
    private async Task<JobFileRecordEntity?> GetSingleActiveJobAsync(
        Expression<Func<JobFileRecordEntity, bool>> predicate, 
        string identifier)
    {
        var job = await _dbSet
            .Where(predicate)
            .Where(job => job.ScheduledAt > DateTimeOffset.UtcNow)
            .SingleOrDefaultAsync();

        if (job is null)
        {
            _logger.LogWarning("{Repo} - No active job found. {Identifier}", nameof(JobFileRecordEntityRepository), identifier);
            return null;
        }

        _logger.LogInformation("{Repo} - Active job found. FileRecordId: {FileRecordId}, JobId: {JobId}.",
            nameof(JobFileRecordEntityRepository), job.FileId, job.Id);

        return job;
    }
}