using Data_Center.Configuration.Database;
using DataCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StorageService.Model.Entities;
using StorageService.Repository.Interface;

namespace DataCenter.Infrastructure.Repository.EntityRepository;

public class LoginAttemptEntityRepository : EntityRepository<LoginAttemptEntity>, ILoginAttemptEntityRepository
{
    private readonly DatabaseContext _dbContext;
    private readonly DbSet<LoginAttemptEntity> _dbSet;
    private readonly ILogger<LoginAttemptEntityRepository> _logger;

    #region Ctor

    public LoginAttemptEntityRepository(DatabaseContext dbContext, ILogger<LoginAttemptEntityRepository> logger) : base(dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<LoginAttemptEntity>();
        _logger = logger;
    }

    #endregion


    public async Task<bool> CheckTrustedIp(string userId, string ipAddress)
    {
        return await _dbSet.AnyAsync(t => t.UserId == userId && t.IpAddress == ipAddress);
    }
}