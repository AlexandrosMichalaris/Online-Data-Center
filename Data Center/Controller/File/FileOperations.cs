using Microsoft.AspNetCore.Mvc;

namespace Data_Center.Controller;

[Route("api/[controller]")]
public class FileOperations : ControllerBase
{
    //Upload
    public async Task<IActionResult> Upload(IFormFile file)
    {
        return Ok();
    }

    //Maybe upload multiple
    
    //Download
    [HttpGet]
    [Route("download/{id}")]
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