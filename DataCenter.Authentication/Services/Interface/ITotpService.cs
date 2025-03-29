namespace DataCenter.Authentication.Services.Interface;

public interface ITotpService
{
    bool VerifyTotpCode(string secretKey, string totpCode);
}