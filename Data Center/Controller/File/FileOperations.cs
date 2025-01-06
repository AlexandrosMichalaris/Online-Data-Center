using Microsoft.AspNetCore.Mvc;

namespace Data_Center.Controller;

[Route("api/[controller]")]
public class FileOperations : ControllerBase
{
    //Upload
    [HttpPost]
    [Route("uploadfile")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }
        
        
        
        return Ok();
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