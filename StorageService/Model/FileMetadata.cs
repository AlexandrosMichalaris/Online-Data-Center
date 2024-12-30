public class FileMetadata
{
    public string FilePath { get; }
    public string FileName { get; }
    public long FileSize { get; }
    public string MimeType { get; }
    public DateTime UploadTime { get; }
    public string StorageFolder { get; }

    public FileMetadata(string filePath, string fileName, long fileSize, string mimeType, DateTime uploadTime, string storageFolder)
    {
        FilePath = filePath;
        FileName = fileName;
        FileSize = fileSize;
        MimeType = mimeType;
        UploadTime = uploadTime;
        StorageFolder = storageFolder;
    }

    public override string ToString()
    {
        return $"File: {FileName}, Size: {FileSize} bytes, Type: {MimeType}, Uploaded: {UploadTime}, Location: {FilePath}";
    }
}