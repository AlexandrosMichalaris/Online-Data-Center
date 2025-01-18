namespace StorageService.Repository.Interface;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task AddMultipleAsync(IEnumerable<T> entities);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}