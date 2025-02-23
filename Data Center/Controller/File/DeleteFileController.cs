using System.Net;
using ApiResponse;
using Microsoft.AspNetCore.Mvc;
using StorageService.Service.Interface;

namespace Data_Center.Controller;

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

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<FileMetadata>>> Delete(int id)
    {
        _logger.LogInformation($"{typeof(DeleteFileController)} - Delete file START.");
        
        var result = await _deleteService.DeleteFileAsync(id);
        
        if (result.Data is null || !result.IsSuccess)
        {
            _logger.LogError($"{nameof(DeleteFileController)} - Delete file FAILED.");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError, 
                new ApiResponse<FileMetadata>(
                    null, 
                    false, 
                    $"Error On Deletion. Result failed with data null."
                )
            );
        }
        
        return new ApiResponse<FileMetadata>(result.Data, "File Deleted successfully. File recover can happen the next 30 days");
    }
    
}