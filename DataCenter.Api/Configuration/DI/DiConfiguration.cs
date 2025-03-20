using Data_Center.Notifications;
using DataCenter.Mapping;
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
        services.AddScoped<IProgressNotifier, ProgressNotifier>();
        services.AddScoped<IDeleteFileService, DeleteFileService>();
        services.AddScoped<IDeleteService, DeleteService>();
        services.AddScoped<IRecoverService, RecoverService>();
        services.AddScoped<IRecoverFileService, RecoverFileService>();
        services.AddScoped<IHangfireJobRepository, HangfireJobRepository>();
        services.AddScoped<IJobFileRecordRepository, JobFileRecordRepository>();
        
        services.AddScoped<ISaveFile, SaveDefaultFileService>();
        services.AddScoped<ISaveFile, SaveDocumentFileService>();
        services.AddScoped<ISaveFile, SaveImageFileService>();
        services.AddScoped<ISaveFileStrategy, SaveSaveFileStrategy>();
        
        // Auto register profiles
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddAutoMapper(typeof(FileRecordProfile)); // points to any profile in that assembly
        services.AddAutoMapper(typeof(JobFileRecordProfile)); // points to any profile in that assembly
    }
}