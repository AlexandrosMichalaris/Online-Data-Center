namespace DataCenter.Authentication.Services.Interface;

public interface IAuthService
{
    Task<string?> AuthenticateUser(string email, string twoFactorCode, string ip);
}