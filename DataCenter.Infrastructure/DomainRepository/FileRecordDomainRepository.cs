using AutoMapper;
using DataCenter.Infrastructure.Repository.DomainRepository.Interface;
using FileProcessing.Model;
using Microsoft.AspNetCore.Http;
using StorageService.Model.Domain;
using StorageService.Repository.Interface;

namespace DataCenter.Infrastructure.Repository.DomainRepository;

public class FileRecordDomainRepository : DomainRepository<FileRecordEntity, FileRecord>, IFileRecordDomainRepository
{
    private readonly IFileRecordEntityRepository _fileRecordEntityRepository;

    #region Ctor

    public FileRecordDomainRepository(
        //IEntityRepository<FileRecordEntity> entityRepository, 
        IMapper mapper, 
        IFileRecordEntityRepository fileRecordEntityRepository) : base(fileRecordEntityRepository, mapper)
    {
        _fileRecordEntityRepository = fileRecordEntityRepository;
    }

    #endregion

    public async Task UpdateStatusAsync(int id, FileStatus status)
    {
        await _fileRecordEntityRepository.UpdateStatusAsync(id, status);
    }

    public async Task RecoverAsync(int id)
    {
        await _fileRecordEntityRepository.RecoverAsync(id);
    }

    public async Task<FileRecord?> GetFileRecordByJobIdAsync(int jobId)
    {
        var entity = await _fileRecordEntityRepository.GetFileRecordByJobIdAsync(jobId);
        return _mapper.Map<FileRecord>(entity);
    }

    public async Task<bool> CheckDuplicateFile(IFormFile file, string computedChecksum)
    {
        return await _fileRecordEntityRepository.CheckDuplicateFile(file, computedChecksum);
    }

    public async Task<IEnumerable<FileRecord>> GetScheduledDeletedFileRecordsAsync()
    {
        var entities = await _fileRecordEntityRepository.GetScheduledDeletedFileRecordsAsync();
        return _mapper.Map<IEnumerable<FileRecord>>(entities);
    }

    public async Task<FileRecord?> GetDeletedFileRecordAsync(int id)
    {
        var entity = await _fileRecordEntityRepository.GetDeletedFileRecordAsync(id);
        return _mapper.Map<FileRecord>(entity);
    }
}