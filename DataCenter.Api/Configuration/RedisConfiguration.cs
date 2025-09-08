using Data_Center.Configuration.Options;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Data_Center.Configuration;

public static class RedisConfiguration
{
    public static void ConfigureRedisService(this WebApplicationBuilder builder)
    {
        builder.Services.AddOptions<RedisOptions>()
            .Bind(builder.Configuration.GetSection("RedisOptions"));

        builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var config = ConfigurationOptions.Parse(
                builder.Configuration.GetConnectionString("Redis"), 
                true);

            var redisOpts = sp.GetRequiredService<IOptions<RedisOptions>>().Value;

            config.AbortOnConnectFail = redisOpts.AbortOnConnectFail;
            config.ConnectRetry = redisOpts.ConnectRetry;
            config.ConnectTimeout = redisOpts.ConnectTimeout;
            config.SyncTimeout = redisOpts.SyncTimeout;
            config.KeepAlive = redisOpts.KeepAlive;
            config.AllowAdmin = redisOpts.AllowAdmin;
            config.DefaultDatabase = redisOpts.DefaultDatabase;

            if (!string.IsNullOrWhiteSpace(redisOpts.Password))
                config.Password = redisOpts.Password;

            if (redisOpts.Ssl)
            {
                config.Ssl = true;
                if (!string.IsNullOrWhiteSpace(redisOpts.SslHost))
                    config.SslHost = redisOpts.SslHost;
            }

            return ConnectionMultiplexer.Connect(config);
        });
    }
}