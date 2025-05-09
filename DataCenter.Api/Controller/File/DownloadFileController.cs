using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.ApiResponse;
using StorageService.Service.Interface;

namespace Data_Center.Controller;

[ApiController]
[Route("api/file_operations/[controller]")]
public class DownloadFileController : ControllerBase
{
    private readonly ILogger<DownloadFileController> _logger;
    private readonly IDownloadService _downloadService;
    
    #region Ctor
    public DownloadFileController(
        IDownloadService downloadService,
        ILogger<DownloadFileController> logger
    )
    {
        _downloadService = downloadService;
        _logger = logger;
    }
    #endregion
    
    /// <summary>
    /// Download File from data center
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize]
    [HttpGet("{id}")]
    [Produces("application/octet-stream")] // is a general-purpose binary download MIME type
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<FileMetadata>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<FileMetadata>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Download(Guid id)
    {
        _logger.LogInformation("{Controller} - Download file START. FileRecordId: {FileId}", nameof(DownloadFileController), id);

        var result = await _downloadService.DownloadFileAsync(id);

        if (!result.IsSuccess)
        {
            var errorMessage = result.ErrorMessage ??
                               (result.StatusCode == (int)HttpStatusCode.NotFound
                                   ? $"File with ID {id} was not found."
                                   : "Unexpected error occurred during file download.");

            _logger.LogWarning("{Controller} - Download file FAILED. FileRecordId: {FileId}, Error: {ErrorMessage}", nameof(DownloadFileController), id, errorMessage);

            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.InternalServerError, new ApiResponse<FileMetadata>(
                data: null,
                success: false,
                message: errorMessage
            ));
        }

        if (result.Data is null)
        {
            var errorMessage = $"Download succeeded but returned no file data for ID {id}.";

            _logger.LogWarning("{Controller} - Download file returned no data. FileRecordId: {FileId}", nameof(DownloadFileController), id);

            return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse<FileMetadata>(
                data: null,
                success: false,
                message: errorMessage
            ));
        }

        _logger.LogInformation("{Controller} - Download file SUCCESS. FileRecordId: {FileId}", nameof(DownloadFileController), id);

        return File(result.Data.Stream, result.Data.FileContentType, result.Data.FileName);
    }
    
    //Download multiple (in .zip)
    [HttpGet("multiple")]
    public async Task<IActionResult> DownloadMultiple([FromBody] IEnumerable<int> ids)
    {
        return Ok();
    }
}