using Microsoft.AspNetCore.Identity;

namespace DataCenter.Domain.Entities;

public class ApplicationUserEntity : IdentityUser
{
    public bool Is2FAEnabled { get; set; } = true;  // Ensures only 2FA login is allowed
    public string? TwoFactorSecretKey { get; set; }  // Google Authenticator secret
    
    public virtual ICollection<TrustedIpEntity> TrustedIps { get; set; } = new List<TrustedIpEntity>();
}