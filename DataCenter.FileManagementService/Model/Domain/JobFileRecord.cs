using StorageService.Model.Entities;

namespace StorageService.Model.Domain;

public class JobFileRecord
{
    public int Id { get; set; }

    public int FileId { get; set; }  // Foreign key to FileRecords

    public long JobId { get; set; } // Foreign key to Hangfire Job table
    
    public string FileName { get; set; }

    public DateTime ScheduledAt { get; set; } = DateTime.UtcNow.AddDays(30);
    
    public virtual FileRecord File { get; set; }
    
    public virtual HangfireJobEntity JobEntity { get; set; }
}