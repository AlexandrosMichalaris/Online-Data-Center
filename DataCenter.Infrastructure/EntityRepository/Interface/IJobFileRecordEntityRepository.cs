using FileProcessing.Model;
using StorageService.Model.Entities;

namespace StorageService.Repository.Interface;

public interface IJobFileRecordEntityRepository : IEntityRepository<JobFileRecordEntity>
{
    Task<IEnumerable<JobFileRecordEntity>> GetFileRecordJobsAsync(Guid fileRecordId);

    Task<JobFileRecordEntity?> GetActiveJobOfFileRecordAsync(string fileName, string checksum);
    
    Task<JobFileRecordEntity?> GetActiveJobOfFileRecordAsync(Guid filerecordId);
    
    Task DeleteJobByRecordIdAsync(Guid recordId);

    List<JobFileRecordEntity> GetPendingJobs();
}