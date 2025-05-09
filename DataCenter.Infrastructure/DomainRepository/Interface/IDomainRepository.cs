namespace DataCenter.Infrastructure.Repository.DomainRepository.Interface;

public interface IDomainRepository<TDomainModel>
    where TDomainModel : class
{
    Task<TDomainModel?> GetByIdAsync(Guid id);
    Task<IEnumerable<TDomainModel>> GetAllAsync();
    Task<TDomainModel> AddAsync(TDomainModel domainModel);
    Task UpdateAsync(TDomainModel domainModel);
    Task DeleteAsync(Guid id);
}