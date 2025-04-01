using DataCenter.Domain.Entities;

namespace StorageService.Repository.Interface;

public interface ILoginAttemptEntityRepository : IEntityRepository<LoginAttemptEntity>
{
    Task<bool> CheckTrustedIp(string userId, string ipAddress);
}