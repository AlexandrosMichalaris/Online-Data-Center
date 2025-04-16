using DataCenter.Domain.Entities;
using DataCenter.Infrastructure.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Data_Center.Configuration.Database;


public class AuthDatabaseContext : IdentityDbContext<ApplicationUserEntity>
{
    private readonly IConfiguration? _configuration;
    
    public DbSet<TrustedIpEntity> TrustedIps { get; set; }
    public DbSet<LoginAttemptEntity> LoginAttempts { get; set; }

    public AuthDatabaseContext(DbContextOptions<AuthDatabaseContext> options) : base(options) { }

    public AuthDatabaseContext(DbContextOptions<AuthDatabaseContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured && _configuration != null)
        {
            var connectionString = _configuration.GetConnectionString("DataCenter");

            var assemblyName = typeof(AuthDatabaseContext).Assembly.GetName().Name;
            optionsBuilder.UseNpgsql(connectionString, b => b.MigrationsAssembly(assemblyName));
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyAuthConfigurations(); // Extension method for entity configurations
        modelBuilder.ApplyGlobalQueryFilters(); // Extension method for global filters
    }
}