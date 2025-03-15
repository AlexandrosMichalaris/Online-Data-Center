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
    private readonly DatabaseContext _dbContext;
    private readonly DbSet<T> _dbSet;
    
    public Repository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
    }
    
    public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

    /// <summary>
    /// Add entry
    /// </summary>
    /// <param name="entity">Entity must be DTO</param>
    public async Task<T> AddAsync(T entity)
    {
        var dbEntity = _dbSet.Add(entity);
        await _dbContext.SaveChangesAsync();
        return dbEntity.Entity;
    }

    /// <summary>
    /// Add multiple
    /// </summary>
    /// <param name="entities">All of them need to be Dtos</param>
    /// <returns></returns>
    public async Task AddMultipleAsync(IEnumerable<T> entities)
    {
        _dbSet.AddRange(entities);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Update 
    /// </summary>
    /// <param name="entity">Needs to be Dto</param>
    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity is not null)
        {
            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}