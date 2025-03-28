using StorageService.Model.Domain;

namespace DataCenter.Infrastructure.Repository.DomainRepository.Interface;

public interface IJobFileRecordDomainRepository : IDomainRepository<JobFileRecord>
{
    Task<IEnumerable<JobFileRecord>> GetFileRecordJobsAsync(int fileRecordId);
    Task<JobFileRecord?> GetActiveJobOfFileRecordAsync(string fileName, string checksum);

    Task<JobFileRecord?> GetActiveJobOfFileRecordAsync(int fileRecordId);
    
    Task DeleteJobByRecordIdAsync(int recordId);
    IEnumerable<JobFileRecord> GetPendingJobs();
}