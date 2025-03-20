using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StorageService.Model.Dto;

namespace Data_Center.Configuration.Database;

public class HangfireStatusConfiguration: IEntityTypeConfiguration<HangfireStateDto>
{
    public void Configure(EntityTypeBuilder<HangfireStateDto> builder)
    {
        builder.ToTable("state", "hangfire");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("id");

        builder.Property(s => s.JobId)
            .HasColumnName("jobid");

        builder.Property(s => s.Name)
            .HasColumnName("name");

        builder.Property(s => s.Reason)
            .HasColumnName("reason");

        builder.Property(s => s.Data)
            .HasColumnName("data");

        builder.Property(s => s.CreatedAt)
            .HasColumnName("createdat");

        // Relationships
        builder.HasOne(s => s.Job)
            .WithMany(j => j.States)
            .HasForeignKey(s => s.JobId);
    }
}