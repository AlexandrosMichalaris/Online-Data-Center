// ModelBuilderExtensions.cs
using System.Linq.Expressions;
using FileProcessing.Model.Interface;
using Microsoft.EntityFrameworkCore;
using StorageService.Model.Domain;

namespace Data_Center.Configuration.Database;

public static class ModelBuilderExtensions
{
    public static void ApplyFileConfigurations(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new FileRecordConfiguration());
        modelBuilder.ApplyConfiguration(new HangfireJobConfiguration());
        modelBuilder.ApplyConfiguration(new JobFileRecordConfiguration());
    }
    
    public static void ApplyAuthConfigurations(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TrustedIpConfiguration());
        modelBuilder.ApplyConfiguration(new LoginAttemptConfiguration());
    }
}