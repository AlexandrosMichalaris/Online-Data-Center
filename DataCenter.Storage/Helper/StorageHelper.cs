using Data_Center.Configuration.Constants;
using StorageService.Extensions;
using StorageService.StorageConstants;

namespace StorageService;

public class StorageHelper
{
    public static (string FileName, string TrashFolder) GetFileNameAndTrashFolder(string filepath)
    {
        var folder = Path.GetDirectoryName(filepath);
        var fileName = Path.GetFileName(filepath);
        var trashFolder = Path.Combine(folder, Constants.TrashFolder);
        return (fileName, trashFolder);
    }

    public static bool FileExists(string filepath)
    {
        return File.Exists(filepath);
    }

    public static string? GetDirectoryName(string filepath)
    {
        return Path.GetDirectoryName(filepath);
    }

    public static string? Combine(string path, string fileName)
    {
        return Path.Combine(path, fileName);
    }
    
    public static int GetBufferSizeFromFileType(FileType fileType)
    {
        return fileType.GetDocumentFileTypes().Contains(fileType) 
            ? Constants.DocumentFileBufferSize
            : fileType.GetImageFileTypes().Contains(fileType)
                ? Constants.ImageFileBufferSize
                : Constants.DefaultFileBufferSize;
    }
}