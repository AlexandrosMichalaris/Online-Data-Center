using ApiResponse;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using StorageService.Service.Interface;

namespace DataCenter.Controller;

[Route("api/[controller]")]
public class FileOperations : ControllerBase
{
    private readonly IFileManagementService _fileManagementService;

    public FileOperations(IFileManagementService fileManagementService)
    {
        _fileManagementService = fileManagementService;
    }
    
    //Upload
    [HttpPost]
    [Route("uploadfile")]
    public async Task<ActionResult<ApiResponse<FileMetadata>>> Upload(IFormFile file)
    {
        if (file.Length == 0)
            return BadRequest(new ApiResponse<FileMetadata>(null, false, "No file uploaded"));
        
        var result = await _fileManagementService.UploadFileAsync(file);
        
        if(result.Data is null)
            return StatusCode(500, new ApiResponse<FileMetadata>(null, false, "Error uploading file, Data result was null."));

        if (!result.IsSuccess)
            return StatusCode(500, new ApiResponse<FileMetadata>(result.Data, false, "Error uploading file."));
        
        return new ApiResponse<FileMetadata>(result.Data, "File uploaded successfully.");
    }

    //Maybe upload multiple
    
    //Download
    [HttpGet]
    [Route("downloadfile/{id}")]
    public async Task<IActionResult> Download(int id)
    {
        return Ok();
    }
    
    //Download multiple (in .zip)
    [HttpPost]
    public async Task<IActionResult> DownloadMultiple([FromBody] IEnumerable<int> ids)
    {
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteMultiple([FromBody] IEnumerable<int> ids)
    {
        return Ok();
    }
}