using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FileProcessing.Model;
using StorageService.Model.Domain;

namespace StorageService.Model.Dto;

public class JobFileRecordDto
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

    // Navigation properties
    [ForeignKey("FileId")]
    public virtual FileRecordDto File { get; set; }

    [ForeignKey("JobId")]
    public virtual HangfireJobDto JobDto { get; set; }
}