using Data_Center.Configuration.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StorageService.Exceptions;
using StorageService.Model.Domain;
using StorageService.Repository.Interface;

namespace StorageService.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ILogger<Repository<T>> _logger;
    private readonly DatabaseContext _dbContext;
    private readonly DbSet<T> _dbSet;
    
    public Repository(DatabaseContext dbContext, ILogger<Repository<T>> logger)
    {
        _logger = logger;
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
    }
    
    public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

    /// <summary>
    /// Add entry
    /// </summary>
    /// <param name="entity">Entity must be DTO</param>
    /// <exception cref="ApplicationException"></exception>
    public async Task<T> AddAsync(T entity)
    {
        try
        {
            var dbEntity = _dbSet.Add(entity);
            await _dbContext.SaveChangesAsync();
            return dbEntity.Entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{typeof(Repository<T>)} - AddAsync - Could not ADD record in Database: {ex.Message}");
            throw new ApplicationException($"{typeof(Repository<T>)} Could not ADD record in Database: {ex.Message}");
        }
    }

    /// <summary>
    /// Add multiple
    /// </summary>
    /// <param name="entities">All of them need to be Dtos</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task AddMultipleAsync(IEnumerable<T> entities)
    {
        try
        {
            _dbSet.AddRange(entities);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{typeof(Repository<T>)} - AddMultipleAsync - Could not ADD multiple records in Database: {ex.Message}");
            throw new ApplicationException($"{typeof(Repository<T>)} Could not ADD multiple records in Database: {ex.Message}");
        }
    }

    /// <summary>
    /// Update 
    /// </summary>
    /// <param name="entity">Needs to be Dto</param>
    /// <exception cref="ApplicationException"></exception>
    public async Task UpdateAsync(T entity)
    {
        try
        {
            _dbSet.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{typeof(Repository<T>)} - UpdateAsync - Could not UPDATE record in Database: {ex.Message}");
            throw new ApplicationException($"{typeof(Repository<T>)} Could not UPDATE record in Database: {ex.Message}");
        }
    }

    public virtual async Task DeleteAsync(int id)
    {
        try
        {
            var entity = await GetByIdAsync(id);
            if (entity is not null)
            {
                _dbSet.Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{typeof(Repository<T>)} - DeleteAsync - Could not REMOVE record in Database: {ex.Message}");
            throw new ApplicationException($"{typeof(Repository<T>)} Could not REMOVE record in Database: {ex.Message}");
        }
    }
}