namespace StorageService.StorageConstants;

public static class Constants
{
    public const int ImageFileBufferSize = 100 * 1024; // 102,400 bytes
    public const int DocumentFileBufferSize = 1024 * 1024; // 1,048,576 bytes
    public const int DefaultFileBufferSize = 1024 * 1024; // 1,048,576 bytes
    
    public const string TrashFolder = "trash";
}