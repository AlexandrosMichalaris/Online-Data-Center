using StorageService.Model.Domain;

namespace DataCenter.Infrastructure.Repository.DomainRepository.Interface;

public interface IJobFileRecordDomainRepository : IDomainRepository<JobFileRecord>
{
    Task<IEnumerable<JobFileRecord>> GetFileRecordJobsAsync(Guid fileRecordId);
    Task<JobFileRecord?> GetActiveJobOfFileRecordAsync(string fileName, string checksum);

    Task<JobFileRecord?> GetActiveJobOfFileRecordAsync(Guid fileRecordId);
    
    Task DeleteJobByRecordIdAsync(Guid recordId);
    IEnumerable<JobFileRecord> GetPendingJobs();
}