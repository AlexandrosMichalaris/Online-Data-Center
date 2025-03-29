using DataCenter.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data_Center.Configuration.Database;


public class AuthDatabaseContext : IdentityDbContext<ApplicationUserEntity>
{
    public DbSet<TrustedIpEntity> TrustedIps { get; set; }
    public DbSet<LoginAttemptEntity> LoginAttempts { get; set; }

    public AuthDatabaseContext(DbContextOptions<AuthDatabaseContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurations(); // Extension method for entity configurations
        modelBuilder.ApplyGlobalQueryFilters(); // Extension method for global filters
    }
}