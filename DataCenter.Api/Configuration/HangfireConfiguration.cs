using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.PostgreSql;

namespace Data_Center.Configuration;

public static class HangfireConfiguration
{
    public static void ConfigureHangfireServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddHangfire(config => config.UseMemoryStorage());
        builder.Services.AddHangfireServer();

        builder.Services.AddHangfire(config =>
            config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DataCenter")));
    }
}