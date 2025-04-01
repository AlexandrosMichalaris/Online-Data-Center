using AutoMapper;
using DataCenter.Domain.Domain;
using DataCenter.Domain.Entities;
using DataCenter.Infrastructure.Repository.DomainRepository.Interface;
using StorageService.Repository.Interface;

namespace DataCenter.Infrastructure.Repository.DomainRepository;

public class LoginAttemptDomainRepository : DomainRepository<LoginAttemptEntity, LoginAttempt>, ILoginAttemptDomainRepository
{
    private readonly ILoginAttemptEntityRepository _loginAttemptEntityRepository;
    
    public LoginAttemptDomainRepository(
        ILoginAttemptEntityRepository entityRepository, 
        IMapper mapper,
        ILoginAttemptEntityRepository loginAttemptEntityRepository) : base(entityRepository, mapper)
    {
        _loginAttemptEntityRepository = loginAttemptEntityRepository;
    }

    public async Task<bool> CheckTrustedIp(string userId, string ipAddress)
    {
        return await _loginAttemptEntityRepository.CheckTrustedIp(userId, ipAddress);
    }
}