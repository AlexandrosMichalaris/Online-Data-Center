using System.Net;
using ApiResponse;
using Microsoft.AspNetCore.Mvc;
using StorageService.Service.Interface;

namespace Data_Center.Controller;

[Route("api/file_operations/[controller]")]
public class RecoverFileController : ControllerBase
{
    private readonly ILogger<RecoverFileController> _logger;
    private readonly IRecoverService _recoverService;

    public RecoverFileController(ILogger<RecoverFileController> logger, IRecoverService recoverService)
    {
        _logger = logger;
        _recoverService = recoverService;
    }


    [HttpPatch("{id}/recover")]
    public async Task<ActionResult<ApiResponse<FileMetadata>>> RecoverFile(int id)
    {
        _logger.LogInformation("{Controller} - Recover file START. FileRecordId: {FileId}", nameof(RecoverFileController), id);

        var result = await _recoverService.RecoverFileAsync(id);

        if (!result.IsSuccess || result.Data is null)
        {
            var errorMessage = result.ErrorMessage ?? $"Failed to recover file with id {id}.";

            _logger.LogWarning("{Controller} - Recover file FAILED. FileRecordId: {FileId}, Error: {ErrorMessage}", nameof(RecoverFileController), id, errorMessage);

            return NotFound(new ApiResponse<FileMetadata>(
                data: null,
                success: false,
                message: errorMessage
            ));
        }

        _logger.LogInformation("{Controller} - Recover file SUCCESS. FileRecordId: {FileId}", nameof(RecoverFileController), id);

        return Ok(new ApiResponse<FileMetadata>(
            data: result.Data,
            success: true,
            message: "File recovered successfully."
        ));
    }
    
}