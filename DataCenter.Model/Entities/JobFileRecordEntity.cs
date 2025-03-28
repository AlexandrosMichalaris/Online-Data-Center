using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FileProcessing.Model;
using StorageService.Model.Domain;

namespace StorageService.Model.Entities;

public class JobFileRecordEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Column("fileid")]
    public int FileId { get; set; }  // Foreign key to FileRecords

    [Required]
    [Column("jobid")]
    public long JobId { get; set; } // Foreign key to Hangfire Job table
    
    [Required]
    [Column("filename", TypeName = "varchar(255)")]
    public string FileName { get; set; }

    [Column("scheduledat")]
    public DateTime ScheduledAt { get; set; } = DateTime.UtcNow.AddDays(30);
    
    [Column("checksum")]
    public string Checksum { get; set; }

    // Navigation properties
    [ForeignKey("FileId")]
    public virtual FileRecordEntity File { get; set; }

    [ForeignKey("JobId")]
    public virtual HangfireJobEntity JobEntity { get; set; }
}