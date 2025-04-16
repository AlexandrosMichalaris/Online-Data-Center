// HangfireJobConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StorageService.Model.Entities;

namespace Data_Center.Configuration.Database;

public class HangfireJobConfiguration : IEntityTypeConfiguration<HangfireJobEntity>
{
    public void Configure(EntityTypeBuilder<HangfireJobEntity> builder)
    {
        builder.ToTable("job", "hangfire");

        builder.HasKey(j => j.Id);

        builder.Property(j => j.Id)
            .HasColumnName("id");

        builder.Property(j => j.StateId)
            .HasColumnName("stateid");

        builder.Property(j => j.StateName)
            .HasColumnName("statename");

        builder.Property(j => j.CreatedAt)
            .HasColumnName("createdat");

        builder.Property(j => j.InvocationData)
            .HasColumnName("invocationdata");

        builder.Property(j => j.Arguments)
            .HasColumnName("arguments");

        builder.Property(j => j.ExpireAt)
            .HasColumnName("expireat");

        // Relationships
        builder.HasMany(j => j.States)
            .WithOne(s => s.Job)
            .HasForeignKey(s => s.JobId);

        builder.HasMany(j => j.JobFileRecords)
            .WithOne(jfr => jfr.JobEntity)
            .HasForeignKey(jfr => jfr.JobId);
    }
}