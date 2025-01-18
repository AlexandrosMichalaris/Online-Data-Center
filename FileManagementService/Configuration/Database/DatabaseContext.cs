using FileProcessing.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Data_Center.Configuration.Database;

public class DatabaseContext : DbContext
{
    
    public readonly IConfiguration _configuration;

    public DatabaseContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    /// <summary>
    /// This ensures the DbContext works even if DI fails to configure it properly.
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured) // Check if already configured
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DataCenter"));
        }
    }
    
    public DbSet<FileRecordDto> FileRecords { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FileRecordDto>(entity =>
        {
            entity.ToTable("FileRecord");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();
            
            entity.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("filename");
            
            entity.Property(e => e.FileType)
                .IsRequired()
                .HasMaxLength(5)
                .HasColumnName("filetype");
            
            entity.Property(e => e.FileSize)
                .IsRequired()
                .HasColumnName("filesize");
            
            entity.Property(e => e.FilePath)
                .HasColumnName("filepath");
            
            entity.Property(e => e.UploadDate)
                .IsRequired()
                .HasColumnType("datetime2(3)") /////////
                .HasColumnName("uploaddate");
            
            entity.Property(e => e.Checksum)
                .HasColumnName("checksum");
            
            entity.Property(e => e.Status)
                .HasConversion<int>()
                .IsRequired();
        });
    }
}