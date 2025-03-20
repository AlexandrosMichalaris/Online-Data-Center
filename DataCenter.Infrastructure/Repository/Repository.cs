using Data_Center.Configuration.Database;
using Microsoft.EntityFrameworkCore;
using StorageService.Repository.Interface;

namespace StorageService.Repository;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly DatabaseContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;
    
    public Repository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<TEntity>();
    }
    
    public async Task<TEntity?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

    public async Task<IEnumerable<TEntity>> GetAllAsync() => await _dbSet.ToListAsync();

    /// <summary>
    /// Add entry
    /// </summary>
    /// <param name="entity">Entity must be DTO</param>
    public async Task<TEntity> AddAsync(TEntity entity)
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
    public async Task AddMultipleAsync(IEnumerable<TEntity> entities)
    {
        _dbSet.AddRange(entities);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Update 
    /// </summary>
    /// <param name="entity">Needs to be Dto</param>
    public async Task UpdateAsync(TEntity entity)
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