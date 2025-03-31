using AutoMapper;
using DataCenter.Domain.Domain;
using DataCenter.Domain.Entities;
using DataCenter.Infrastructure.Repository.DomainRepository.Interface;
using StorageService.Repository.Interface;

namespace DataCenter.Infrastructure.Repository.DomainRepository;

public class LoginAttemptDomainRepository : DomainRepository<LoginAttemptEntity, LoginAttempt>, ILoginAttemptDomainRepository
{
    public LoginAttemptDomainRepository(IEntityRepository<LoginAttemptEntity> entityRepository, IMapper mapper) : base(entityRepository, mapper)
    {
    }
}