using DataCenter.Domain.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.ApiResponse;
using StorageService.Service.Interface;

namespace Data_Center.Controller;

[ApiController]
[Route("api/[controller]")]
public class FileInfoController : ControllerBase
{
    private readonly IFileInfoService _fileInfoService;
    private readonly ILogger<FileInfoController> _logger;

    public FileInfoController(IFileInfoService fileRecordService, ILogger<FileInfoController> logger)
    {
        _fileInfoService = fileRecordService;
        _logger = logger;
    }

    [Authorize]
    [HttpGet]
    [Route("retrieve/paged")]
    public Task<ActionResult<ApiResponse<IEnumerable<FileRecordMetadata>>>> GetPagedFileRecords(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        return GetPagedFileRecordsInternal(page, pageSize, isDeleted: false);
    }

    [Authorize]
    [HttpGet]
    [Route("retrieve/deleted/paged")]
    public Task<ActionResult<ApiResponse<IEnumerable<FileRecordMetadata>>>> GetDeletedPagedFileRecords(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        return GetPagedFileRecordsInternal(page, pageSize, isDeleted: true);
    }

    private async Task<ActionResult<ApiResponse<IEnumerable<FileRecordMetadata>>>> GetPagedFileRecordsInternal(
        int page, int pageSize, bool isDeleted)
    {
        var type = isDeleted ? "deleted" : "active";
        _logger.LogInformation($"Received request to fetch {type} paged file records. Page: {page}, PageSize: {pageSize}");

        var result = await _fileInfoService.GetPagedFileRecordsAsync(page, pageSize, isDeleted);

        if (!result.IsSuccess)
        {
            _logger.LogWarning($"Failed to fetch {type} paged file records. Reason: {result.ErrorMessage}");
            return BadRequest(new ApiResponse<IEnumerable<FileRecordMetadata>>(
                data: null,
                success: false,
                message: result.ErrorMessage
            ));
        }

        return Ok(new ApiResponse<IEnumerable<FileRecordMetadata>>(
            data: result.Data,
            success: true,
            message: result.ErrorMessage ?? $"Paged {type} file records retrieved successfully."
        ));
    }
}