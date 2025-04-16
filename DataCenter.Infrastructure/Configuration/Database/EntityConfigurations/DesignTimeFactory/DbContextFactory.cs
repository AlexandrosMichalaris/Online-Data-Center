using DataCenter.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Data_Center.Configuration.Database.DesignTimeFactory;

/// <summary>
/// This is used only by EF tooling to migrate the tables in database
/// </summary>
public class DbContextFactory: IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        var configPath = Path.Combine(Directory.GetCurrentDirectory(), "..", Constants.ConfigProject);

        // Load configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(configPath)
            .AddJsonFile(Constants.ConfigFile)
            .Build();
        
        return new DatabaseContext(configuration);
    }
}