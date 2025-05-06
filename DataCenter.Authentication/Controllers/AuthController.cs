using DataCenter.Authentication.Services.Interface;
using DataCenter.Domain.Dto;
using Microsoft.AspNetCore.Mvc;
using Model.ApiResponse;

namespace DataCenter.Authentication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<string>>> Login([FromBody] LoginDto model)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var (token, errorMessage) = await _authService.AuthenticateUser(model.Email, model.TwoFactorCode, model.Password, ip);

        if (token == null)
        {
            return Unauthorized(new ApiResponse<string>(null, false, errorMessage ?? "Authentication failed."));
        }

        return Ok(new ApiResponse<string>(token, true, "Login successful."));
    }
}