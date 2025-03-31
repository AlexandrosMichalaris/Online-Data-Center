using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.ApiResponse;
using StorageService.Service.Interface;

namespace Data_Center.Controller;

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
    public async Task<ActionResult<ApiResponse<FileMetadata>>> Download(int id)
    {
        _logger.LogInformation("{Controller} - Download file START. FileRecordId: {FileId}", nameof(DownloadFileController), id);

        var result = await _downloadService.DownloadFileAsync(id);

        if (!result.IsSuccess || result.Data is null)
        {
            var errorMessage = result.ErrorMessage ?? $"Failed to download file with id {id}.";

            _logger.LogWarning("{Controller} - Download file FAILED. FileRecordId: {FileId}, Error: {ErrorMessage}", nameof(DownloadFileController), id, errorMessage);

            return NotFound(new ApiResponse<FileMetadata>(
                data: null,
                success: false,
                message: errorMessage
            ));
        }

        _logger.LogInformation("{Controller} - Download file SUCCESS. FileRecordId: {FileId}", nameof(DownloadFileController), id);

        return File(result.Data.Stream, result.Data.FileContentType, result.Data.FileName);
    }
    
    //Download multiple (in .zip)
    [HttpGet]
    public async Task<IActionResult> DownloadMultiple([FromBody] IEnumerable<int> ids)
    {
        return Ok();
    }
    
    [HttpGet]
    public async Task<IActionResult> Preview([FromBody] IEnumerable<int> ids)
    {
        return Ok();
    }
}