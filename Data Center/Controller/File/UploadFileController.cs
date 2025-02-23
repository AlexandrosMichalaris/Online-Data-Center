using System.Net;
using ApiResponse;
using Microsoft.AspNetCore.Mvc;
using StorageService.Service.Interface;

namespace Data_Center.Controller;

[Route("api/file_operations")]
public class UploadFileController : ControllerBase
{
    private readonly ILogger<DownloadFileController> _logger;
    private readonly IUploadService _uploadService;
    
    #region Ctor
    public UploadFileController(
        IUploadService uploadService,
        ILogger<DownloadFileController> logger
    )
    {
        _uploadService = uploadService;
        _logger = logger;
    }
    #endregion
    
    /// <summary>
    /// Upload file controller
    /// </summary>
    /// <param name="file"></param>
    /// <param name="connectionId">Connection id for signalR to show the percentage.</param>
    /// <returns></returns>
    [HttpPost]
    [Route("[controller]")]
    public async Task<ActionResult<ApiResponse<FileMetadata>>> Upload([FromForm]IFormFile file, [FromQuery] string connectionId = "")
    {
        _logger.LogInformation("Upload file START");
        
        if (file.Length == 0)
            return BadRequest(new ApiResponse<FileMetadata>(null, false, "No file uploaded"));
        
        var result = await _uploadService.UploadFileAsync(file, connectionId);
        
        if (!result.IsSuccess)
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.InternalServerError, new ApiResponse<FileMetadata>(result.Data, false, result.ErrorMessage ?? 
                (result.StatusCode == (int)HttpStatusCode.BadRequest ? "Error uploading file. The data you provided is not valid." : 
                    "Error uploading file. Result failed.")));
        
        if(result.Data is null)
            return StatusCode(
                (int)HttpStatusCode.InternalServerError, new ApiResponse<FileMetadata>(null, false, result.ErrorMessage ?? "Error uploading file, Data result was null."));

        return new ApiResponse<FileMetadata>(result.Data, "File uploaded successfully.");
    }
}