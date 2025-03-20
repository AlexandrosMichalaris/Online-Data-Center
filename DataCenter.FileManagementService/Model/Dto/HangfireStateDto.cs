using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StorageService.Model.Domain;

namespace StorageService.Model.Dto;

[Table("state", Schema = "hangfire")]
public class HangfireStateDto
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
    public DateTime CreatedAt { get; set; }

    // Navigation to HangfireJob
    [ForeignKey(nameof(JobId))]
    public virtual HangfireJobDto Job { get; set; }

    // Navigation to JobFileRecord by JobId
    public virtual JobFileRecordDto JobFileRecord { get; set; }
}