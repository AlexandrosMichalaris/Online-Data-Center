using Data_Center.Configuration.Database;
using FileProcessing.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StorageService.Model.Dto;
using StorageService.Repository.Interface;

namespace StorageService.Repository;

public class JobFileRecordRepository : Repository<JobFileRecordDto>, IJobFileRecordRepository
{
    private readonly DatabaseContext _dbContext;
    private readonly DbSet<JobFileRecordDto> _dbSet;
    private readonly ILogger<JobFileRecordRepository> _logger;
    
    public JobFileRecordRepository(DatabaseContext dbContext, ILogger<JobFileRecordRepository> logger) : base(dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<JobFileRecordDto>();
        _logger = logger;
    }

    public async Task<IEnumerable<JobFileRecordDto>> GetFileRecordJobsAsync(int fileRecordId)
    {
        return await _dbSet
            .Where(job => job.FileId == fileRecordId)
            .ToListAsync();
    }

    public async Task<JobFileRecordDto?> GetActiveJobOfFileRecordAsync(int fileRecordId)
    {
        var jobs = await _dbSet
            .Where(job => job.FileId == fileRecordId && job.ScheduledAt > DateTimeOffset.Now)
            .ToListAsync();

        if (jobs.Count != 1)
        {
            _logger.LogError(
                $"{nameof(JobFileRecordRepository)} - GetActiveJobsOfFileRecordAsync - returned jobs amount is not 1.");
            throw new InvalidOperationException(
                $"{nameof(JobFileRecordRepository)} - GetActiveJobsOfFileRecordAsync - returned jobs amount is not 1.");
        }

        return jobs.First();
    }

    public async Task DeleteJobByRecordIdAsync(int recordId)
    {
        var job = await _dbSet.Where(job => job.FileId == recordId).FirstOrDefaultAsync();

        if (job is not null)
        {
            _dbSet.Remove(job);
            await _dbContext.SaveChangesAsync();
        }
    }
    
    public List<JobFileRecordDto> GetPendingJobs()
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
                j => j.hjob.Id,
                state => state.JobId,
                (j, states) => new { j.jfr, latestState = states.OrderByDescending(s => s.CreateDate).FirstOrDefault() }
            )
            .Where(j => j.latestState != null && 
                        (j.latestState.Name == "Scheduled" || j.latestState.Name == "Enqueued"))
            .Select(j => j.jfr)
            .ToList();
    }
}