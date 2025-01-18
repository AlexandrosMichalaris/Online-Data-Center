using Data_Center.Configuration.Database;
using Microsoft.EntityFrameworkCore;
using StorageService.Model.Domain;
using StorageService.Repository.Interface;

namespace StorageService.Repository;

/// <summary>
/// File Record Repository
/// Every operation done here, is considered to be done into the storage too.
/// </summary>
public class FileRecordRepository : Repository<FileRecord>, IFileRecordRepository
{
    private readonly DatabaseContext _dbContext;
    private readonly DbSet<FileRecord> _dbSet;
    
    public FileRecordRepository(DatabaseContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<FileRecord>();
    }
    

    public async Task<string> GetFilePathAsync(int id)
    {
        try
        {
            var record = await _dbSet.FindAsync(id);

            if (record is null)
                throw new ApplicationException($"Record with id {id} not found");
            
            return await Task.FromResult(record.FilePath);
        }
        catch (Exception e)
        {
            throw new ApplicationException($"{typeof(FileRecordRepository)} Could not GET FILE PATH of record in Database: {e.Message}");
        }
    }

    public async Task UpdateStatusAsync(int id, FileStatus status)
    {
        try
        {
            var record = await _dbSet.FindAsync(id);

            if (record is not null)
            {
                record.Status = status;
                await _dbContext.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            throw new ApplicationException($"Status of Record with id {id} could not be updated: {e.Message}");
        }
    }

    //The path is going to include the file itself. That's why it's distinct.
    public async Task UpdateStatusAsync(string filePath, FileStatus status)
    {
        try
        {
            var records = _dbSet.Where(record => record.FilePath == filePath).ToList();

            if (!records.Any() || records.Count > 1)
                throw new ApplicationException($"Multiple records with the same filepath {filePath} exist.");
            
            records.First().Status = status;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new ApplicationException($"Status of Record with filepath {filePath} could not be updated: {e.Message}");
        }
    }
}