using DataCenter.Authentication.Services.Interface;
using OtpNet;

namespace DataCenter.Authentication.Services;

public class TotpService : ITotpService
{
    public bool VerifyTotpCode(string secretKey, string totpCode)
    {
        var keyBytes = Base32Encoding.ToBytes(secretKey);
        var totp = new Totp(keyBytes);

        return totp.VerifyTotp(totpCode, out _, VerificationWindow.RfcSpecifiedNetworkDelay);
    }
}