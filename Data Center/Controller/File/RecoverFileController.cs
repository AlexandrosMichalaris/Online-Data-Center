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
        var result = await _recoverService.RecoverFileAsync(id);
        
        if (!result.IsSuccess || result.Data is null)
        {
            _logger.LogError($"{nameof(RecoverFileController)} - Recover file FAILED.");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError, 
                new ApiResponse<FileMetadata>(
                    result.Data, 
                    false, 
                    $"Error On Recover. Result failed with data null."
                )
            );
        }
        
        return new ApiResponse<FileMetadata>(result.Data, "File Recovered successfully.");
    }
    
}