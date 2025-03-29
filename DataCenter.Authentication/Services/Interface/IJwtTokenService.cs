using DataCenter.Domain.Entities;

namespace DataCenter.Authentication.Services.Interface;

public interface IJwtTokenService
{
    string GenerateToken(ApplicationUserEntity userEntity);
}