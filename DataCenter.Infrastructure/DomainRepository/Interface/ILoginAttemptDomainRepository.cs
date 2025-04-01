using DataCenter.Domain.Domain;

namespace DataCenter.Infrastructure.Repository.DomainRepository.Interface;

public interface ILoginAttemptDomainRepository : IDomainRepository<LoginAttempt>
{
    Task<bool> CheckTrustedIp(string userId, string ipAddress);
}