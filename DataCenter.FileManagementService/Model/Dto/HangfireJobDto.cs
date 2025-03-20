using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StorageService.Model.Domain;

namespace StorageService.Model.Dto;

// Model for Hangfire Job table
[Table("job", Schema = "hangfire")]
public class HangfireJobDto
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("stateid")]
    public int? StateId { get; set; }

    [Column("statename")]
    public string StateName { get; set; }

    [Column("createdat")]
    public DateTime CreatedAt { get; set; }

    [Column("invocationdata")]
    public string InvocationData { get; set; }

    [Column("arguments")]
    public string Arguments { get; set; }

    [Column("expiresat")]
    public DateTime? ExpiresAt { get; set; }

    // Navigation property to Hangfire States
    public virtual ICollection<HangfireStateDto> States { get; set; }

    // Navigation property to your JobFileRecord entity
    public virtual ICollection<JobFileRecordDto> JobFileRecords { get; set; }
}