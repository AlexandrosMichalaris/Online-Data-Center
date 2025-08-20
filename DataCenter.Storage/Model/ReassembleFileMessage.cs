using Data_Center.Configuration.Constants;

namespace DataCenter.Domain.Dto;

public class ReassembleFileMessage
{
    public Guid FileId;

    public int TotalChunks;
    
    public FileType FileType;
    
    public FolderType FolderType;
}