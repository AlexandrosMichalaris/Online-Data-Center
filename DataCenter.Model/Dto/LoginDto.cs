namespace DataCenter.Domain.Dto;

public class LoginDto
{
    public string Email { get; set; }
    public string TwoFactorCode { get; set; }
    
    public string Password { get; set; }
}