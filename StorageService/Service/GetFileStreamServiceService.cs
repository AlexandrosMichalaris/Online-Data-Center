using Data_Center.Configuration;
using Microsoft.Extensions.Options;
using StorageService.Exceptions;
using StorageService.Service.Interface;

namespace StorageService.Service;

public class GetFileStreamServiceService : IGetFileStreamService
{
    private readonly FileStorageOptions _options;
    
    public GetFileStreamServiceService(IOptions<FileStorageOptions> options)
    {
        _options = options.Value;
    }

    public async Task<FileResultGeneric<Stream>> GetFileStream(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return FileResultGeneric<Stream>.Failure("File not found.");
            }

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return FileResultGeneric<Stream>.Success(fileStream);
        }
        catch (Exception ex)
        {
            throw new StorageException<Stream>($"Failed to retrieve file: {ex.Message}");
        }
    }
}