using StorageService;
using StorageService.Service;
using StorageService.Service.Interface;

namespace Data_Center.Configuration.DI;

public static class DiConfiguration
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IFileManagementService, FileManagementService>();

        services.AddScoped<ISaveFile, DefaultFileHandler>();
        services.AddScoped<ISaveFile, DocumentFileHandler>();
        services.AddScoped<ISaveFile, ImageFileHandler>();
    }
}