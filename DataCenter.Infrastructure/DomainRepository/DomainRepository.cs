using AutoMapper;
using DataCenter.Infrastructure.Repository.DomainRepository.Interface;
using StorageService.Repository.Interface;

namespace DataCenter.Infrastructure.Repository.DomainRepository;

public class DomainRepository<TEntity, TDomainModel> : IDomainRepository<TDomainModel>
    where TEntity : class
    where TDomainModel : class
{
    protected readonly IEntityRepository<TEntity> _entityRepository;
    protected readonly IMapper _mapper;

    public DomainRepository(IEntityRepository<TEntity> entityRepository, IMapper mapper)
    {
        _entityRepository = entityRepository;
        _mapper = mapper;
    }

    public async Task<TDomainModel?> GetByIdAsync(int id)
    {
        var entity = await _entityRepository.GetByIdAsync(id);
        return _mapper.Map<TDomainModel>(entity);
    }

    public async Task<IEnumerable<TDomainModel>> GetAllAsync()
    {
        var entities = await _entityRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<TDomainModel>>(entities);
    }

    public async Task<TDomainModel> AddAsync(TDomainModel domainModel)
    {
        var entity = _mapper.Map<TEntity>(domainModel);
        var addedEntity = await _entityRepository.AddAsync(entity);
        return _mapper.Map<TDomainModel>(addedEntity);
    }

    public async Task UpdateAsync(TDomainModel domainModel)
    {
        var entity = _mapper.Map<TEntity>(domainModel);
        await _entityRepository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(int id)
    {
        await _entityRepository.DeleteAsync(id);
    }
}