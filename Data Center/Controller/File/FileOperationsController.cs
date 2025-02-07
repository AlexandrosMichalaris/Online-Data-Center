using System.Net;
using ApiResponse;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using StorageService.Service.Interface;

namespace Data_Center.Controller;

[Route("api/[controller]")]
public class FileOperationsController : ControllerBase
{
    private readonly ILogger<FileOperationsController> _logger;
    private readonly IUploadService _uploadService;
    private readonly IDownloadService _downloadService;

    public FileOperationsController(
        IUploadService uploadService,
        IDownloadService downloadService,
        ILogger<FileOperationsController> logger
    )
    {
        _uploadService = uploadService;
        _downloadService = downloadService;
        _logger = logger;
    }
    
    //Upload
    [HttpPost]
    [Route("uploadfile")]
    public async Task<ActionResult<ApiResponse<FileMetadata>>> Upload([FromForm]IFormFile file)
    {
        _logger.LogInformation("Upload file START");
        
        if (file.Length == 0)
            return BadRequest(new ApiResponse<FileMetadata>(null, false, "No file uploaded"));
        
        var result = await _uploadService.UploadFileAsync(file);
        
        if (!result.IsSuccess)
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.InternalServerError, new ApiResponse<FileMetadata>(result.Data, false, result.ErrorMessage ?? 
                (result.StatusCode == (int)HttpStatusCode.BadRequest ? "Error uploading file. The data you provided is not valid." : 
                    "Error uploading file. Result failed.")));
        
        if(result.Data is null)
            return StatusCode(
                (int)HttpStatusCode.InternalServerError, new ApiResponse<FileMetadata>(null, false, result.ErrorMessage ?? "Error uploading file, Data result was null."));

        return new ApiResponse<FileMetadata>(result.Data, "File uploaded successfully.");
    }

    //Maybe upload multiple
    
    //Download
    [HttpGet]
    [Route("downloadfile/{id}")]
    public async Task<IActionResult> Download(int id)
    {
        return Ok();
    }
    
    //Download multiple (in .zip)
    [HttpPost]
    public async Task<IActionResult> DownloadMultiple([FromBody] IEnumerable<int> ids)
    {
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteMultiple([FromBody] IEnumerable<int> ids)
    {
        return Ok();
    }
}