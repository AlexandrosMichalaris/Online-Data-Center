using System.Net;
using ApiResponse;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    [HttpPost]
    [Route("[controller]")]
    public async Task<ActionResult<ApiResponse<FileMetadata>>> Upload([FromForm]IFormFile file, [FromQuery] string connectionId = "")
    {
        _logger.LogInformation("{Controller} - Upload file START. FileName: {FileName}", nameof(UploadFileController), file?.FileName);

        if (file == null || file.Length == 0)
        {
            _logger.LogWarning("{Controller} - Upload file FAILED. No file was uploaded.", nameof(UploadFileController));
        
            return BadRequest(new ApiResponse<FileMetadata>(
                data: null,
                success: false,
                message: "No file uploaded."
            ));
        }

        var result = await _uploadService.UploadFileAsync(file, connectionId);

        if (!result.IsSuccess)
        {
            var errorMessage = result.ErrorMessage ?? 
                               (result.StatusCode == (int)HttpStatusCode.BadRequest
                                   ? "Invalid input data provided during upload."
                                   : "Unexpected error uploading the file.");

            _logger.LogWarning("{Controller} - Upload file FAILED. FileName: {FileName}, Error: {ErrorMessage}", nameof(UploadFileController), file.FileName, errorMessage);

            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.InternalServerError, new ApiResponse<FileMetadata>(
                data: null,
                success: false,
                message: errorMessage
            ));
        }

        if (result.Data is null)
        {
            var errorMessage = "Upload succeeded but returned no data.";

            _logger.LogWarning("{Controller} - Upload file returned no data. FileName: {FileName}", nameof(UploadFileController), file.FileName);

            return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse<FileMetadata>(
                data: null,
                success: false,
                message: errorMessage
            ));
        }

        _logger.LogInformation("{Controller} - Upload file SUCCESS. FileName: {FileName}", nameof(UploadFileController), file.FileName);

        return Ok(new ApiResponse<FileMetadata>(
            data: result.Data,
            success: true,
            message: "File uploaded successfully."
        ));
    }
}