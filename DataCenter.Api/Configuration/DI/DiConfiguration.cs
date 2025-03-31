using Data_Center.Notifications;
using DataCenter.Authentication.Services;
using DataCenter.Authentication.Services.Interface;
using DataCenter.Infrastructure.Repository.DomainRepository;
using DataCenter.Infrastructure.Repository.DomainRepository.Interface;
using DataCenter.Infrastructure.Repository.EntityRepository;
using DataCenter.Mapping;
using StorageService;
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
        services.AddScoped<ICheckSumService, CheckSumService>();
        services.AddScoped<IGetFileStreamService, GetFileStreamService>();
        services.AddScoped<IProgressNotifier, ProgressNotifier>();
        services.AddScoped<IDeleteFileService, DeleteFileService>();
        services.AddScoped<IDeleteService, DeleteService>();
        services.AddScoped<IRecoverService, RecoverService>();
        services.AddScoped<IRecoverFileService, RecoverFileService>();
        
        services.AddScoped<IHangfireJobEntityRepository, HangfireJobEntityRepository>();
        services.AddScoped<IJobFileRecordEntityRepository, JobFileRecordEntityRepository>();
        services.AddScoped<IFileRecordEntityRepository, FileRecordEntityRepository>();
        services.AddScoped<ILoginAttemptEntityRepository, LoginAttemptEntityRepository>();
        
        services.AddScoped<IHangfireJobDomainRepository, HangfireJobDomainRepository>();
        services.AddScoped<IJobFileRecordDomainRepository, JobFileRecordDomainRepository>();
        services.AddScoped<IFileRecordDomainRepository, FileRecordDomainRepository>();
        services.AddScoped<ILoginAttemptDomainRepository, ILoginAttemptDomainRepository>();
        
        services.AddScoped<ISaveFile, SaveDefaultFileService>();
        services.AddScoped<ISaveFile, SaveDocumentFileService>();
        services.AddScoped<ISaveFile, SaveImageFileService>();
        services.AddScoped<ISaveFileStrategy, SaveSaveFileStrategy>();
        
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ITotpService, TotpService>();
        
        // Auto register profiles
        services.AddAutoMapper(typeof(FileRecordProfile)); // points to any profile in that assembly
        services.AddAutoMapper(typeof(JobFileRecordProfile)); // points to any profile in that assembly
        services.AddAutoMapper(typeof(LoginAttemptProfile));
        services.AddAutoMapper(typeof(TrustedIpProfile));
    }
}