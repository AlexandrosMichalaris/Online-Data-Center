using Data_Center.Configuration.Database;
using Microsoft.EntityFrameworkCore;
using StorageService.Repository.Interface;

namespace DataCenter.Infrastructure.Repository.EntityRepository;

public class EntityRepository<TEntity> : IEntityRepository<TEntity> where TEntity : class
{
    private readonly DatabaseContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;
    
    public EntityRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<TEntity>();
    }
    

    public async Task<TEntity?> GetByIdAsync(int id) => 
        await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);

    public async Task<IEnumerable<TEntity>> GetAllAsync() => 
        await _dbSet.AsNoTracking().ToListAsync();

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
    /// Update Async. Load the tracked entity from the DB and apply changes to it
    /// </summary>
    /// <param name="entity"></param>
    public async Task UpdateAsync(TEntity entity)
    {
        //  Get keys of entity
        var keyValues = _dbContext.Entry(entity).Metadata.FindPrimaryKey()
            ?.Properties.Select(p => p.PropertyInfo.GetValue(entity)).ToArray();

        var existingEntity = await _dbSet.FindAsync(keyValues);

        if (existingEntity == null)
        {
            throw new InvalidOperationException("Entity not found in the database.");
        }

        // Apply changes from the passed-in entity to the tracked one
        _dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);

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