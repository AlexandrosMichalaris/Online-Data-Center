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
            var connectionString = _configuration.GetConnectionString("DataCenter");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DataCenter' is missing.");
            }
            optionsBuilder.UseNpgsql(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurations(); // Extension method for entity configurations
        modelBuilder.ApplyGlobalQueryFilters(); // Extension method for global filters
    }
}