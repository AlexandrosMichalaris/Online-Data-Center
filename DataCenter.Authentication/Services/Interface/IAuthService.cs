namespace DataCenter.Authentication.Services.Interface;

public interface IAuthService
{
    Task<(string? Token, string? ErrorMessage)> AuthenticateUser(string email, string twoFactorCode, string ip);
}