using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using StorageService.Service.Interface;

namespace StorageService.Service;

public class CheckSumService : ICheckSumService
{
    public async Task<string> ComputeChecksumAsync(IFormFile file)
    {
        if (file is null)
        {
            throw new ArgumentNullException(nameof(file));
        }

        using (var sha256 = SHA256.Create())
        using (var stream = file.OpenReadStream())
        {
            byte[] hashBytes = await sha256.ComputeHashAsync(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }
    
    public async Task<bool> VerifyChecksumAsync(string computedChecksum, string expectedChecksum)
    {
        return string.Equals(computedChecksum, expectedChecksum, StringComparison.OrdinalIgnoreCase);
    }

    public async Task<bool> VerifyChecksumAsync(IFormFile file, string expectedChecksum)
    {
        string computedChecksum = await ComputeChecksumAsync(file);
        return string.Equals(computedChecksum, expectedChecksum, StringComparison.OrdinalIgnoreCase);
    }
}