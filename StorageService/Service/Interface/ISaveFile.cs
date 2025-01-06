using Data_Center.Configuration.Constants;
using Microsoft.AspNetCore.Http;

namespace StorageService.Service.Interface;

public interface ISaveFile
{
    public IEnumerable<FileType> FileTypes { get; }
    
    public FolderType FolderType { get; }
    
    public Task<FileStorageResult<FileMetadata>> SaveFileAsync(IFormFile file, string basePath);
    
    public Task<FileStorageResult<Stream>> GetFileStream(string filePath);
}


// TODO: Decorator to check each accepted size.

// public async Task<IActionResult> UploadFile(IFormFile file)
// {
//     if (file == null || file.Length == 0)
//     {
//         return BadRequest("No file uploaded.");
//     }
//
//     // Validate file type
//     var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
//     var fileExtension = Path.GetExtension(file.FileName).ToLower();
//
//     if (!allowedExtensions.Contains(fileExtension))
//     {
//         return BadRequest("Unsupported file type.");
//     }
//
//     // Limit file size to 5 MB
//     if (file.Length > 5 * 1024 * 1024)
//     {
//         return BadRequest("File size exceeds the limit.");
//     }
//
//     // Save the file
//     var filePath = await _storageService.SaveFileAsync(file);
//
//     return Ok(new { Path = filePath });
// }