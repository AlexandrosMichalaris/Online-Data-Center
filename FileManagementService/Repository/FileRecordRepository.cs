using Data_Center.Configuration.Database;
using FileProcessing.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StorageService.Model.Domain;
using StorageService.Repository.Interface;

namespace StorageService.Repository;

/// <summary>
/// File Record Repository
/// Every operation done here, is considered to be done into the storage too.
/// </summary>
public class FileRecordRepository : Repository<FileRecordDto>, IFileRecordRepository
{
    private readonly DatabaseContext _dbContext;
    private readonly DbSet<FileRecordDto> _dbSet;
    private readonly ILogger<FileRecordRepository> _logger;
    
    public FileRecordRepository(DatabaseContext dbContext, ILogger<FileRecordRepository> logger) : base(dbContext, logger)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<FileRecordDto>();
        _logger = logger;
    }
    

    public async Task<string> GetFilePathAsync(int id)
    {
        try
        {
            var record = await _dbSet.FindAsync(id);
            
            if (record is null)
            {
                _logger.LogError($"{nameof(FileRecordRepository)} - GetFilePathAsync - Record with id {id} not found");
                throw new ApplicationException($"{nameof(FileRecordRepository)} - GetFilePathAsync - Record with id {id} not found");
            }
            
            return await Task.FromResult(record.FilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(FileRecordRepository)} - GetFilePathAsync - Could not GET FILE PATH of record in Database: {ex.Message}");
            throw new ApplicationException($"{nameof(FileRecordRepository)} Could not GET FILE PATH of record in Database: {ex.Message}");
        }
    }

    public async Task UpdateStatusAsync(int id, FileStatus status)
    {
        try
        {
            var record = await _dbSet.FindAsync(id);

            if (record is not null)
            {
                record.Status = (int)status;
                await _dbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(FileRecordRepository)} - UpdateStatusAsync - Status of Record with id {id} could not be updated: {ex.Message}");
            throw new ApplicationException($"{nameof(FileRecordRepository)} Status of Record with id {id} could not be updated: {ex.Message}");
        }
    }


    public override async Task DeleteAsync(int id)
    {
        try
        {
            var entity = await GetByIdAsync(id);
            if (entity is not null)
            {
                entity.IsDeleted = true;
                await _dbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(FileRecordRepository)} - DeleteAsync - Could not REMOVE record in Database: {ex.Message}");
            throw new ApplicationException($"{nameof(FileRecordRepository)} Could not REMOVE record in Database: {ex.Message}");
        }
    }

    public async Task<bool> CheckDuplicateFile(IFormFile file, string computedChecksum)
    {
        // Check against database
        return await _dbSet.AnyAsync(f => 
            f.Checksum == computedChecksum 
            //&& f.IsDeleted == false
        );
    }
}