using StorageService.Service.Interface;

namespace Data_Center.Configuration.DI;

public static class DiConfiguration
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IFileManagementService>();

        services.AddScoped<ISaveFile>();
    }
}