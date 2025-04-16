using DataCenter.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Data_Center.Configuration.Database.DesignTimeFactory;

/// <summary>
/// This is used only by EF tooling to migrate the tables in database
/// </summary>
public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDatabaseContext>
{
    public AuthDatabaseContext CreateDbContext(string[] args)
    {
        var configPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "DataCenter.Api");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(configPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DataCenter");

        var optionsBuilder = new DbContextOptionsBuilder<AuthDatabaseContext>();

        var migrationsAssembly = typeof(AuthDatabaseContext).Assembly.GetName().Name;

        optionsBuilder.UseNpgsql(connectionString, b => b.MigrationsAssembly(migrationsAssembly));

        return new AuthDatabaseContext(optionsBuilder.Options, configuration);
    }
}