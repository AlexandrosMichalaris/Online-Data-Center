using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DataCenter.Authentication.Services;
using DataCenter.Authentication.Services.Interface;
using DataCenter.Domain.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DataCenter.Authentication.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var token = await _authService.AuthenticateUser(model.Email, model.TwoFactorCode, ip);

        if (token == null)
            return Unauthorized("Invalid 2FA or IP not trusted.");

        return Ok(new { Token = token });
    }
}