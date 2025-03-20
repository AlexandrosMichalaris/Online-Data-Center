using FileProcessing.Model;
using StorageService.Model.Entities;

namespace StorageService.Repository.Interface;

public interface IJobFileRecordRepository : IRepository<JobFileRecordEntity>
{
    Task<IEnumerable<JobFileRecordEntity>> GetFileRecordJobsAsync(int fileRecordId);
    
    Task<JobFileRecordEntity?> GetActiveJobOfFileRecordAsync(int fileRecordId);
    
    Task DeleteJobByRecordIdAsync(int recordId);

    List<JobFileRecordEntity> GetPendingJobs();
}