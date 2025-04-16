using DataCenter.Domain.Entities;
using DataCenter.Infrastructure.Configuration;
using FileProcessing.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StorageService.Model.Entities;

namespace Data_Center.Configuration.Database;

public class DatabaseContext : DbContext
{
    private readonly IConfiguration _configuration;

    public DatabaseContext(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public DbSet<FileRecordEntity> FileRecords { get; set; }
    public DbSet<JobFileRecordEntity> JobFileRecords { get; set; }
    public DbSet<HangfireJobEntity> HangfireJobs { get; set; }
    
    public DbSet<HangfireStateEntity> HangfireStates { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "DataCenter.Api"); // adjust as needed
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = configuration.GetConnectionString("DataCenter");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DataCenter' is missing.");
            }

            // Use this context's assembly for migrations
            var assemblyName = typeof(AuthDatabaseContext).Assembly.GetName().Name;

            optionsBuilder.UseNpgsql(connectionString, b => b.MigrationsAssembly(assemblyName));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyFileConfigurations(); // Extension method for entity configurations
        modelBuilder.ApplyGlobalQueryFilters(); // Extension method for global filters
    }
}