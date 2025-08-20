using Data_Center.Configuration.Constants;

namespace DataCenter.Domain.Dto;

public class ReassembleFileMessage
{
    public Guid FileId { get; set; }
    
    public string FileName { get; set; } = null!;

    public int TotalChunks { get; set; }
    
    public FileType FileType { get; set; }
    
    public FolderType FolderType { get; set; }
}