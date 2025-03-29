using Data_Center.Configuration.Database;
using DataCenter.Authentication.Services.Interface;
using DataCenter.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DataCenter.Authentication.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUserEntity> _userManager;
    private readonly IJwtTokenService _jwtService;
    private readonly AuthDatabaseContext _dbContext;
    private readonly ITotpService _totpService;

    public AuthService(UserManager<ApplicationUserEntity> userManager, IJwtTokenService jwtService, AuthDatabaseContext dbContext, ITotpService totpService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _dbContext = dbContext;
        _totpService = totpService;
    }

    public async Task<string?> AuthenticateUser(string email, string twoFactorCode, string ip)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !user.Is2FAEnabled || string.IsNullOrEmpty(user.TwoFactorSecretKey))
            return null;

        // Validate TOTP
        bool isValidTotp = _totpService.VerifyTotpCode(user.TwoFactorSecretKey, twoFactorCode);
        if (!isValidTotp)
            return null; // 

        // Check Trusted IP
        bool isTrustedIp = await _dbContext.TrustedIps.AnyAsync(t => t.UserId == user.Id && t.IpAddress == ip);
        if (!isTrustedIp)
            return null; /// 

        // Generate JWT if all checks pass
        return _jwtService.GenerateToken(user);
    }
}