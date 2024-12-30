using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Data_Center.Controller;

[Route("api/[controller]")]
public class FileInfo : ControllerBase
{
    //TODO: File identification is going to change
    [HttpGet]
    [Route("info/{id}")]
    public async Task<IActionResult> GetFileInfo(int id)
    {
        return Ok();
    }
    
    
}