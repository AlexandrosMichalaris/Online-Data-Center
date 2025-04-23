using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StorageService.Model.Domain;

namespace StorageService.Model.Entities;

[Table("state", Schema = "hangfire")]
public class HangfireStateEntity
{
    [Key]
    public int Id { get; set; } 

    [Required]
    public long JobId { get; set; } // FK to HangfireJob

    [Required]
    public string Name { get; set; }

    public string Reason { get; set; }

    public string Data { get; set; } // JSON blob with ScheduledAt, EnqueueAt

    [Column("createdat")]
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation to HangfireJob
    [ForeignKey(nameof(JobId))]
    public virtual HangfireJobEntity Job { get; set; }

    // Navigation to JobFileRecord by JobId
    [NotMapped]
    public virtual JobFileRecordEntity JobFileRecord { get; set; }
}