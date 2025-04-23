using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StorageService.Model.Entities;

namespace Data_Center.Configuration.Database;

public class JobFileRecordConfiguration: IEntityTypeConfiguration<JobFileRecordEntity>
{
    public void Configure(EntityTypeBuilder<JobFileRecordEntity> builder)
    {
        builder.ToTable("jobfilerecords");
        
        builder.HasKey(x => x.Id);

        builder.Property(e => e.Id)
            //.ValueGeneratedOnAdd()
            .HasColumnName("id");

        builder.Property(e => e.FileId)
            .IsRequired()
            .HasColumnName("fileid");

        builder.Property(e => e.JobId)
            .IsRequired()
            .HasColumnName("jobid");

        builder.Property(e => e.FileName)
            .IsRequired()
            .HasColumnName("filename");
        
        builder.Property(e => e.ScheduledAt)
            .HasColumnName("scheduledat");
        
        builder.HasOne(e => e.File)
            .WithMany(f => f.JobFileRecords)  // assumes you have ICollection<JobFileRecordDto> in FileRecord
            .HasForeignKey(e => e.FileId)
            .HasPrincipalKey(f => f.Id)       // <-- explicitly linking to FileRecord.Id
            .OnDelete(DeleteBehavior.Cascade); // or whatever behavior fits

        // Relationship to HangfireJobDto
        builder.HasOne(e => e.JobEntity)
            .WithMany(h => h.JobFileRecords) // assumes ICollection<JobFileRecordDto> in HangfireJobDto
            .HasForeignKey(e => e.JobId)
            .HasPrincipalKey(h => h.Id)      // explicitly linking to HangfireJobDto.Id
            .OnDelete(DeleteBehavior.Cascade);
    }
}