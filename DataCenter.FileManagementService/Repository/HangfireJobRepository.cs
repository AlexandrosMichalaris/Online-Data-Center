using Data_Center.Configuration.Database;
using Microsoft.Extensions.Logging;
using StorageService.Model.Dto;

namespace StorageService.Repository.Interface;

public class HangfireJobRepository : Repository<HangfireJobDto>, IHangfireJobRepository
{
    public HangfireJobRepository(DatabaseContext dbContext) : base(dbContext) { }
}