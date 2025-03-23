using AutoMapper;
using DataCenter.Infrastructure.Repository.DomainRepository.Interface;
using StorageService.Model.Domain;
using StorageService.Model.Entities;
using StorageService.Repository;
using StorageService.Repository.Interface;

namespace DataCenter.Infrastructure.Repository.DomainRepository;

public class JobFileRecordDomainRepository : DomainRepository<JobFileRecordEntity, JobFileRecord>, IJobFileRecordDomainRepository
{
    private readonly IJobFileRecordEntityRepository _jobFileRecordEntityRepository;
    
    public JobFileRecordDomainRepository(
        //IEntityRepository<JobFileRecordEntity> entityRepository, 
        IMapper mapper,
        IJobFileRecordEntityRepository jobFileRecordEntityRepository
        ) : base(jobFileRecordEntityRepository, mapper)
    {
        _jobFileRecordEntityRepository = jobFileRecordEntityRepository;
    }

    public async Task<IEnumerable<JobFileRecord>> GetFileRecordJobsAsync(int fileRecordId)
    {
        var entities = await _jobFileRecordEntityRepository.GetFileRecordJobsAsync(fileRecordId);
        return _mapper.Map<IEnumerable<JobFileRecord>>(entities);
    }

    public async Task<JobFileRecord?> GetActiveJobOfFileRecordAsync(int fileRecordId)
    {
        var entity = await _jobFileRecordEntityRepository.GetActiveJobOfFileRecordAsync(fileRecordId);
        return _mapper.Map<JobFileRecord>(entity);
    }

    public async Task DeleteJobByRecordIdAsync(int recordId)
    {
        await _jobFileRecordEntityRepository.DeleteJobByRecordIdAsync(recordId);
    }

    public IEnumerable<JobFileRecord> GetPendingJobs()
    {
        var entities = _jobFileRecordEntityRepository.GetPendingJobs();
        return _mapper.Map<List<JobFileRecord>>(entities);
    }
}