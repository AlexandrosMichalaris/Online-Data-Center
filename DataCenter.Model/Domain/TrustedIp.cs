namespace DataCenter.Domain.Domain;

public class TrustedIp
{
    public Guid Id { get; set; }
    
    public string IpAddress { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}