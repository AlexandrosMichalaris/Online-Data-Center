using Data_Center.Configuration.Database;
using DataCenter.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Data_Center.Configuration;

public static class DatabaseContextConfiguration
{
    public static void ConfigureDatabaseContextServices(this WebApplicationBuilder builder)
    {
        // Configure Db context's 
        builder.Services.AddDbContext<DatabaseContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DataCenter")));

        builder.Services.AddDbContext<AuthDatabaseContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DataCenter")));

        builder.Services.AddIdentity<ApplicationUserEntity, IdentityRole>()
            .AddEntityFrameworkStores<AuthDatabaseContext>()
            .AddDefaultTokenProviders();
    }
}