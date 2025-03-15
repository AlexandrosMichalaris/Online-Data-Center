using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StorageService.Model.Dto;

namespace Data_Center.Configuration.Database;

public class JobFileRecordConfiguration: IEntityTypeConfiguration<JobFileRecordDto>
{
    public void Configure(EntityTypeBuilder<JobFileRecordDto> builder)
    {
        builder.ToTable("JobFileRecords");

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("Id");

        builder.Property(e => e.FileId)
            .IsRequired()
            .HasColumnName("FileId");

        builder.Property(e => e.JobId)
            .IsRequired()
            .HasColumnName("JobId");

        builder.Property(e => e.FileName)
            .IsRequired()
            .HasColumnName("FileName");
        
        builder.Property(e => e.ScheduledAt)
            .HasColumnName("ScheduledAt");
    }
}