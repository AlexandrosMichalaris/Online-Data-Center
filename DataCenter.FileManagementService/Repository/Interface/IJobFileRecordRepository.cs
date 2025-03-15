using FileProcessing.Model;
using StorageService.Model.Dto;

namespace StorageService.Repository.Interface;

public interface IJobFileRecordRepository : IRepository<JobFileRecordDto>
{
    Task<IEnumerable<JobFileRecordDto>> GetFileRecordJobsAsync(int fileRecordId);
    
    Task<JobFileRecordDto?> GetActiveJobOfFileRecordAsync(int fileRecordId);
    
    Task DeleteJobByRecordIdAsync(int recordId);

    List<JobFileRecordDto> GetPendingJobs();
}