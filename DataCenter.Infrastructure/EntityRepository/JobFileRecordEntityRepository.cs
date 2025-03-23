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
    
    public JobFileRecordEntityRepository(DatabaseContext dbContext, ILogger<JobFileRecordEntityRepository> logger) : base(dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<JobFileRecordEntity>();
        _logger = logger;
    }

    public async Task<IEnumerable<JobFileRecordEntity>> GetFileRecordJobsAsync(int fileRecordId)
    {
        return await _dbSet
            .Where(job => job.FileId == fileRecordId)
            .ToListAsync();
    }

    public async Task<JobFileRecordEntity?> GetActiveJobOfFileRecordAsync(int fileRecordId)
    {
        var activeJobs = await _dbSet
            .Where(job => job.FileId == fileRecordId && job.ScheduledAt > DateTimeOffset.UtcNow)
            .ToListAsync();

        if (activeJobs.Count != 1)
        {
            var errorMessage = $"{nameof(JobFileRecordEntityRepository)} - GetActiveJobOfFileRecordAsync - Expected exactly 1 active job but found {activeJobs.Count}. FileRecordId: {fileRecordId}";

            _logger.LogError(errorMessage);

            throw new InvalidOperationException(errorMessage);
        }

        _logger.LogInformation("{Repo} - Active job found for FileRecordId: {FileRecordId}, JobId: {JobId}.",
            nameof(JobFileRecordEntityRepository), fileRecordId, activeJobs.First().Id);

        return activeJobs.First();
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
}