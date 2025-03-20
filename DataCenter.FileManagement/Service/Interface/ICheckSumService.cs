using Microsoft.AspNetCore.Http;

namespace StorageService.Service.Interface;

public interface ICheckSumService
{
    public Task<string> ComputeChecksumAsync(IFormFile file);
    
    public Task<bool> VerifyChecksumAsync(string computedChecksum, string expectedChecksum);

    public Task<bool> VerifyChecksumAsync(IFormFile file, string expectedChecksum);
}