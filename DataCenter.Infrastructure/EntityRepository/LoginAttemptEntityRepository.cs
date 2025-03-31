using Data_Center.Configuration.Database;
using DataCenter.Domain.Entities;
using StorageService.Repository.Interface;

namespace DataCenter.Infrastructure.Repository.EntityRepository;

public class LoginAttemptEntityRepository : EntityRepository<LoginAttemptEntity>, ILoginAttemptEntityRepository
{
    public LoginAttemptEntityRepository(DatabaseContext dbContext) : base(dbContext)
    {
    }
}