using Data_Center.Configuration.Database;
using Microsoft.Extensions.Logging;
using StorageService.Model.Entities;
using StorageService.Repository.Interface;

namespace DataCenter.Infrastructure.Repository.EntityRepository;

public class HangfireJobEntityRepository : EntityRepository<HangfireJobEntity>, IHangfireJobEntityRepository
{
    public HangfireJobEntityRepository(DatabaseContext dbContext) : base(dbContext) { }
}