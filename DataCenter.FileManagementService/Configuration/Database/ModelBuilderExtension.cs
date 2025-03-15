// ModelBuilderExtensions.cs
using System.Linq.Expressions;
using FileProcessing.Model.Interface;
using Microsoft.EntityFrameworkCore;
using StorageService.Model.Domain;

namespace Data_Center.Configuration.Database;

public static class ModelBuilderExtensions
{
    public static void ApplyConfigurations(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new FileRecordConfiguration());
        modelBuilder.ApplyConfiguration(new HangfireJobConfiguration());
        modelBuilder.ApplyConfiguration(new JobFileRecordConfiguration());
    }

    public static void ApplyGlobalQueryFilters(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var isDeletedProperty = Expression.Property(parameter, nameof(IDeletable.IsDeleted));
                var statusProperty = Expression.Property(parameter, "Status");

                var isDeletedCondition = Expression.Equal(isDeletedProperty, Expression.Constant(false));
                var statusCondition = Expression.Equal(statusProperty, Expression.Constant(FileStatus.Completed));
                var combinedCondition = Expression.AndAlso(isDeletedCondition, statusCondition);

                var filter = Expression.Lambda(combinedCondition, parameter);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }
    }
}