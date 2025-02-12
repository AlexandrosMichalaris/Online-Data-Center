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
        services.AddScoped<IUploadService, UploadService>();
        services.AddScoped<IDownloadService, DownloadService>();
        services.AddScoped<IFileRecordRepository, FileRecordRepository>();
        services.AddScoped<ICheckSumService, CheckSumService>();
        services.AddScoped<IGetFileStreamService, GetFileStreamService>();
        
        services.AddScoped<IFileRecordRepository, FileRecordRepository>();
        
        services.AddScoped<ISaveFile, SaveDefaultFileService>();
        services.AddScoped<ISaveFile, SaveDocumentFileService>();
        services.AddScoped<ISaveFile, SaveImageFileService>();
        services.AddScoped<ISaveFileStrategy, SaveSaveFileStrategy>();
    }
}