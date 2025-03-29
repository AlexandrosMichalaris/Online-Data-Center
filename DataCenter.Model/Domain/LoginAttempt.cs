namespace DataCenter.Domain.Domain;

public class LoginAttempt
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    
    public string IpAddress { get; set; } = string.Empty;
    
    public bool Success { get; set; }  // true = successful, false = failed
    
    public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;
}