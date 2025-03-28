using FileProcessing.Model;
using StorageService.Model.Entities;

namespace StorageService.Repository.Interface;

public interface IJobFileRecordEntityRepository : IEntityRepository<JobFileRecordEntity>
{
    Task<IEnumerable<JobFileRecordEntity>> GetFileRecordJobsAsync(int fileRecordId);

    Task<JobFileRecordEntity?> GetActiveJobOfFileRecordAsync(string fileName, string checksum);
    
    Task<JobFileRecordEntity?> GetActiveJobOfFileRecordAsync(int filerecordId);
    
    Task DeleteJobByRecordIdAsync(int recordId);

    List<JobFileRecordEntity> GetPendingJobs();
}