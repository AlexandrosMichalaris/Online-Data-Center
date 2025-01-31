using Microsoft.EntityFrameworkCore;
using StorageService;
using StorageService.Repository;
using StorageService.Repository.Interface;
using StorageService.Service;
using StorageService.Service.Interface;
using StorageService.Service.Strategy;

namespace Data_Center.Configuration.DI;

public static class DiConfiguration
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IFileManagementService, FileManagementService>();
        services.AddScoped<IFileRecordRepository, FileRecordRepository>();
        services.AddScoped<ICheckSumService, CheckSumService>();
        
        services.AddScoped<IFileRecordRepository, FileRecordRepository>();
        
        services.AddScoped<ISaveFile, DefaultFileHandler>();
        services.AddScoped<ISaveFile, DocumentFileHandler>();
        services.AddScoped<ISaveFile, ImageFileHandler>();
        services.AddScoped<IFileHandlerStrategy, FileHandlerStrategy>();
    }
}