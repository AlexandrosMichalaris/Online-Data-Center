using FileProcessing.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Data_Center.Configuration.Database;

public class DatabaseContext : DbContext
{
    public DbSet<FileRecordDto> FileRecords { get; set; }
    
    public readonly IConfiguration _configuration;

    public DatabaseContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DataCenter"));
    }

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
            
        });
    }
    
    
    
}