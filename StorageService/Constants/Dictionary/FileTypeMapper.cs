using Data_Center.Configuration.Constants;

namespace StorageService.Service;

public class FileTypeMapper
{
    private static readonly Dictionary<string, FileType> MimeTypeMap = new()
    {
        // Text Files
        { "text/plain", FileType.Txt },
        { "text/csv", FileType.Csv },
        { "application/json", FileType.Json },
        { "application/xml", FileType.Xml },
        { "text/html", FileType.Html },
        { "text/markdown", FileType.Markdown },

        // Image Files
        { "image/jpeg", FileType.Jpg },  // Jpg and Jpeg can map to the same type if needed
        { "image/png", FileType.Png },
        { "image/gif", FileType.Gif },

        // Audio Files
        { "audio/mpeg", FileType.Mp3 },
        { "audio/wav", FileType.Wav },

        // Document Files
        { "application/pdf", FileType.Pdf },
        { "application/msword", FileType.Doc },
        { "application/vnd.openxmlformats-officedocument.wordprocessingml.document", FileType.Docx },
        { "application/vnd.ms-excel", FileType.Xls },
        { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileType.Xlsx },
        { "application/vnd.ms-powerpoint", FileType.Ppt },
        { "application/vnd.openxmlformats-officedocument.presentationml.presentation", FileType.Pptx },

        // Compressed Files
        { "application/zip", FileType.Zip },
        { "application/x-rar-compressed", FileType.Rar }
    };

    public static FileType GetFileTypeFromContentType(string contentType)
    {
        if (string.IsNullOrEmpty(contentType))
            return FileType.Unknown;

        return MimeTypeMap.TryGetValue(contentType.ToLowerInvariant(), out var fileType)
            ? fileType
            : FileType.Unknown;
    }

    public static string GetContentTypeFromFileType(FileType fileType)
    {
        // Get the first one that matches. Values do not necessarily have to be unique, so we have to do a lookup. 
        return MimeTypeMap.FirstOrDefault(x => x.Value == fileType).Key;
    }
}