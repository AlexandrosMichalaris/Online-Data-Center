using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataCenter.Domain.Entities;

public class TrustedIpEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string IpAddress { get; set; } = string.Empty;

    [Required]
    public string UserId { get; set; } = string.Empty;
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("UserId")]
    public virtual ApplicationUserEntity UserEntity { get; set; }
}