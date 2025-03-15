using FileProcessing.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StorageService.Model.Dto;

namespace Data_Center.Configuration.Database;

public class DatabaseContext : DbContext
{
    private readonly IConfiguration _configuration;

    public DatabaseContext(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public DbSet<FileRecordDto> FileRecords { get; set; }
    public DbSet<JobFileRecordDto> JobFileRecords { get; set; }
    public DbSet<HangfireJobDto> HangfireJobs { get; set; }
    
    public DbSet<HangfireStateDto> HangfireStates { get; set; }

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