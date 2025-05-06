using Microsoft.AspNetCore.Http;

namespace DataCenter.Domain.Dto;

/// <summary>
/// This was created to serve the swagger doc generator
/// </summary>
public class UploadFileRequestDto
{
    public IFormFile File { get; set; }
}