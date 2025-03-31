using Data_Center.Configuration.Database;
using DataCenter.Authentication.Services.Interface;
using DataCenter.Domain.Domain;
using DataCenter.Domain.Entities;
using DataCenter.Infrastructure.Repository.DomainRepository.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DataCenter.Authentication.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUserEntity> _userManager;
    private readonly IJwtTokenService _jwtService;
    private readonly AuthDatabaseContext _dbContext;
    private readonly ITotpService _totpService;
    private readonly ILoginAttemptDomainRepository _loginAttemptDomainRepository;

    #region Ctor

    public AuthService(
        UserManager<ApplicationUserEntity> userManager, 
        IJwtTokenService jwtService, 
        AuthDatabaseContext dbContext, 
        ITotpService totpService,
        ILoginAttemptDomainRepository loginAttemptDomainRepository)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _dbContext = dbContext;
        _totpService = totpService;
        _loginAttemptDomainRepository = loginAttemptDomainRepository;
    }

    #endregion

    public async Task<(string? Token, string? ErrorMessage)> AuthenticateUser(string email, string twoFactorCode, string ip)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            await LogFailedLogin(ip, "User not found.");
            return (null, "Invalid email or 2FA.");
        }

        if (!user.Is2FAEnabled || string.IsNullOrEmpty(user.TwoFactorSecretKey))
        {
            await LogFailedLogin(ip, "2FA not enabled.");
            return (null, "2FA is not set up for this account.");
        }

        // Validate TOTP
        if (!_totpService.VerifyTotpCode(user.TwoFactorSecretKey, twoFactorCode))
        {
            await LogFailedLogin(ip, "Invalid 2FA code.");
            return (null, "Invalid 2FA code.");
        }

        // Check Trusted IP
        bool isTrustedIp = await _dbContext.TrustedIps.AnyAsync(t => t.UserId == user.Id && t.IpAddress == ip);
        if (!isTrustedIp)
        {
            await LogFailedLogin(ip, "IP not trusted.");
            return (null, "Your IP address is not trusted.");
        }

        // Generate JWT
        var token = _jwtService.GenerateToken(user);
        return (token, null);
    }

// Logs failed login attempt
    private async Task LogFailedLogin(string ip, string reason)
    {
        var loginAttempt = new LoginAttempt
        {
            IpAddress = ip,
            AttemptedAt = DateTime.UtcNow,
            Success = false,
            Reason = reason
        };
        
        await _loginAttemptDomainRepository.AddAsync(loginAttempt);
    }
}