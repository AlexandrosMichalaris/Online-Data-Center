using Data_Center.Configuration.Constants;

namespace DataCenter.Domain.Domain;

public class FileChunk
{
    public Guid FileId { get; set; } = Guid.Empty;

    public string FileName { get; set; } = null!;
    
    public FileType FileType { get; set; }
    
    public FolderType FolderType { get; set; }
    
    public int ChunkNumber { get; set; }
    
    public int TotalChunks { get; set; }
    
    public string? DiskId { get; set; }
    
    public string? Path { get; set; }
    
    public byte[] Data { get; set; } = null!;
}