using FileProcessing.Model;

namespace StorageService.Model.Domain;

public class FileRecord
{
    public int Id { get; set; }  // Primary key for the entity
    
    public string FileName { get; set; }  // Name of the file
    
    public string FileType { get; set; }  // MIME type or extension (e.g., .jpg, .pdf)
    
    public long FileSize { get; set; }  // Size of the file in bytes
    
    public string FilePath { get; set; }  // Path to the file (e.g., file system path or URL)
    
    public DateTime UploadDate { get; set; }  // Date of file upload
    
    public string Checksum { get; set; }  // Optional: Hash of the file for validation

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
            Checksum = this.Checksum
        };
    }
}