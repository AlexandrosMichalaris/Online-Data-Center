using FileProcessing.Model;

namespace StorageService.Model.Domain;

public class FileRecord
{
    public int Id { get; set; }
    
    /// <summary>
    /// Name of the file
    /// </summary>
    public string FileName { get; set; }
    
    /// <summary>
    /// MIME type or extension (e.g., .jpg, .pdf)
    /// </summary>
    public string FileType { get; set; }
    
    /// <summary>
    /// Size of the file in bytes
    /// </summary>
    public long FileSize { get; set; }
    
    /// <summary>
    /// Path to the file (e.g., file system path or URL)
    /// </summary>
    public string FilePath { get; set; }
    
    /// <summary>
    /// Date of file upload
    /// </summary>
    public DateTime UploadDate { get; set; }
    
    /// <summary>
    /// Last day that changes were made
    /// </summary>
    public DateTime UpdateDate { get; set; }
    
    /// <summary>
    /// Optional: Hash of the file for validation
    /// </summary>
    public string? Checksum { get; set; }
    
    /// <summary>
    /// Status of file. (Pending, Completed, Failed)
    /// </summary>
    public FileStatus Status { get; set; }
    
    /// <summary>
    /// If file is deleted
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    public FileRecordDto ToDto()
    {
        return new FileRecordDto()
        {
            Id = this.Id,
            FileName = this.FileName,
            FileType = this.FileType,
            FileSize = this.FileSize,
            FilePath = this.FilePath,
            UploadDate = this.UploadDate,
            Checksum = this.Checksum,
            Status = this.Status,
            IsDeleted = this.IsDeleted
        };
    }
}