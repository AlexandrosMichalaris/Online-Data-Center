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
    private readonly PasswordHasher<ApplicationUserEntity> _passwordHasher;
    private readonly ITotpService _totpService;
    private readonly ILoginAttemptDomainRepository _loginAttemptDomainRepository;

    #region Ctor

    public AuthService(
        UserManager<ApplicationUserEntity> userManager, 
        IJwtTokenService jwtService, 
        ITotpService totpService,
        ILoginAttemptDomainRepository loginAttemptDomainRepository,
        PasswordHasher<ApplicationUserEntity> passwordHasher)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _totpService = totpService;
        _loginAttemptDomainRepository = loginAttemptDomainRepository;
        _passwordHasher = passwordHasher;
    }

    #endregion

    public async Task<(string? Token, string? ErrorMessage)> AuthenticateUser(string email, string twoFactorCode, string password, string ip)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            await LogFailedLogin(null, ip, "User not found.");
            return (null, "Invalid email or 2FA.");
        }
        
        // var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        //
        // if (passwordResult == PasswordVerificationResult.Failed)
        // {
        //     await LogFailedLogin(null, ip, "Wrong password.");
        //     return (null, "Wrong password.");
        // }

        // Validate TOTP
        if (!user.Is2FAEnabled || string.IsNullOrEmpty(user.TwoFactorSecretKey) || !_totpService.VerifyTotpCode(user.TwoFactorSecretKey, twoFactorCode))
        {
            await LogFailedLogin(user.Id, ip, "Invalid 2FA code.");
            return (null, "Invalid 2FA code.");
        }

        // Check Trusted IP
        var isTrustedIp = await _loginAttemptDomainRepository.CheckTrustedIp(user.Id, ip);
        if (!isTrustedIp)
        {
            await LogFailedLogin(user.Id, ip, "IP not trusted.");
            return (null, "Your IP address is not trusted.");
        }

        // Generate JWT
        var token = _jwtService.GenerateToken(user);
        return (token, null);
    }

// Logs failed login attempt
    private async Task LogFailedLogin(string? userId, string ip, string reason)
    {
        var loginAttempt = new LoginAttempt
        {
            UserId = userId,
            IpAddress = ip,
            AttemptedAt = DateTime.UtcNow,
            Success = false,
            Reason = reason
        };
        
        await _loginAttemptDomainRepository.AddAsync(loginAttempt);
    }
}