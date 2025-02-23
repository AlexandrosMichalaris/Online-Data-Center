using ApiResponse;
using Microsoft.AspNetCore.Mvc;
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
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<FileMetadata>>> Download(int id)
    {
        _logger.LogInformation($"{typeof(DownloadFileController)} - Download file START.");
        
        var result = await _downloadService.DownloadFileAsync(id);
        
        if (result.Data is null || !result.IsSuccess)
        {
            _logger.LogError($"{nameof(DownloadFileController)} - Download file FAILED.");
            return NotFound(result.ErrorMessage);
        }

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