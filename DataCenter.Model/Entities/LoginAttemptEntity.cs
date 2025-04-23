using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataCenter.Domain.Entities;

public class LoginAttemptEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string? UserId { get; set; } = string.Empty;

    [ForeignKey("UserId")]
    public virtual ApplicationUserEntity? UserEntity { get; set; }

    [Required]
    public string IpAddress { get; set; } = string.Empty;

    [Required]
    public bool Success { get; set; }  // true = successful, false = failed
    
    [Required]
    public string? Reason { get; set; } = string.Empty;

    [Required]
    public DateTime AttemptAt { get; set; } = DateTime.UtcNow;
}