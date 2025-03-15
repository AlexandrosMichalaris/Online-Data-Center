using FileProcessing.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data_Center.Configuration.Database;

public class FileRecordConfiguration : IEntityTypeConfiguration<FileRecordDto>
{
    public void Configure(EntityTypeBuilder<FileRecordDto> builder)
    {
        builder.ToTable("filerecord");

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");

        builder.Property(e => e.FileName)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("filename");

        builder.Property(e => e.FileType)
            .IsRequired()
            .HasMaxLength(5)
            .HasColumnName("filetype");

        builder.Property(e => e.FileSize)
            .IsRequired()
            .HasColumnName("filesize");

        builder.Property(e => e.FilePath)
            .HasColumnName("filepath");

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.Property(e => e.Checksum)
            .HasColumnName("checksum");

        builder.Property(e => e.IsDeleted)
            .IsRequired()
            .HasColumnName("isDeleted");

        builder.Property(e => e.Status)
            .HasConversion<int>()
            .IsRequired()
            .HasColumnName("status");
        
        // Conflict with HasDefaultValueSql("NOW()") in Model Definition
        // EF Core tracks created_at and updated_at, but the trigger also modifies them, EF Core overwrites what PostgreSQL does.
        // Here we are ignoring the properties in EF Core
        builder.Property(e => e.CreatedAt).ValueGeneratedOnAdd();
        builder.Property(e => e.UpdatedAt).ValueGeneratedOnAddOrUpdate();

        // Add an index for better filtering performance
        builder.HasIndex(e => e.IsDeleted);
    }
}