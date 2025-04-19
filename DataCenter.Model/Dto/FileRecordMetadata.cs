namespace DataCenter.Domain.Dto;

public class FileRecordMetadata
{
    public int Id { get; set; }
    
    public string FileName { get; set; } = string.Empty;
    
    public string ContentType { get; set; } = string.Empty;
    
    public long Size { get; set; } // in bytes
    
    public string Folder { get; set; } = string.Empty;
    
    public DateTimeOffset? CreatedAt { get; set; }
    
    public DateTimeOffset? UpdatedAt { get; set; }
}