using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.ApiResponse;
using StorageService.Service.Interface;

namespace Data_Center.Controller;

[ApiController]
[Route("api/file_operations/[controller]")]
public class DeleteFileController : ControllerBase
{
    private readonly ILogger<DeleteFileController> _logger;
    private readonly IDeleteService _deleteService;

    #region Ctor

    public DeleteFileController(
        IDeleteService deleteService, 
        ILogger<DeleteFileController> logger)
    {
        _deleteService = deleteService;
        _logger = logger;
    }

    #endregion

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<FileMetadata>>> Delete(Guid id)
    {
        _logger.LogInformation("{Controller} - Delete file START. FileRecordId: {FileId}", nameof(DeleteFileController), id);

        var result = await _deleteService.DeleteFileAsync(id);

        if (!result.IsSuccess)
        {
            var errorMessage = result.ErrorMessage ?? $"Failed to delete file with id {id}.";
        
            _logger.LogWarning("{Controller} - Delete file FAILED. FileRecordId: {FileId}, Error: {ErrorMessage}", nameof(DeleteFileController), id, errorMessage);

            return NotFound(new ApiResponse<FileMetadata>(
                data: null,
                success: false,
                message: errorMessage
            ));
        }

        if (result.Data is null)
        {
            var errorMessage = $"Delete operation succeeded but returned no data. FileRecordId: {id}";

            _logger.LogWarning("{Controller} - Delete file returned no data. FileRecordId: {FileId}", nameof(DeleteFileController), id);

            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                new ApiResponse<FileMetadata>(
                    data: null,
                    success: false,
                    message: errorMessage
                )
            );
        }

        _logger.LogInformation("{Controller} - Delete file SUCCESS. FileRecordId: {FileId}", nameof(DeleteFileController), id);

        return Ok(new ApiResponse<FileMetadata>(
            data: result.Data,
            success: true,
            message: "File deleted successfully. Recovery is available for 30 days."
        ));
    }
}