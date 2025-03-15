using StorageService.Model.Domain;

namespace StorageService.Model;

public class StreamData
{
    public string FileName { get; set; }
    
    public string FileContentType { get; set; }
    
    public Stream Stream { get; set; }
}