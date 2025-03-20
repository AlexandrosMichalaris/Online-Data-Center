using Data_Center.Configuration.Database;
using Microsoft.Extensions.Logging;
using StorageService.Model.Entities;

namespace StorageService.Repository.Interface;

public class HangfireJobRepository : Repository<HangfireJobEntity>, IHangfireJobRepository
{
    public HangfireJobRepository(DatabaseContext dbContext) : base(dbContext) { }
}