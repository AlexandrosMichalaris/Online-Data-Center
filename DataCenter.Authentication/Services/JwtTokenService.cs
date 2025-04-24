using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DataCenter.Authentication.Services.Interface;
using DataCenter.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DataCenter.Authentication.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _config;

    public JwtTokenService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(ApplicationUserEntity userEntity)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userEntity.UserName),
            new Claim(ClaimTypes.NameIdentifier, userEntity.Id)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddHours(4),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}