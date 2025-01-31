using Data_Center.Configuration.Constants;

namespace StorageService.Extensions;

public static class FileTypeExtension
{
    public static IEnumerable<FileType> GetDefaultFileTypes(this FileType fileType)
    {
        return new[]
        {
            FileType.Txt,
            FileType.Csv,
            FileType.Json,
            FileType.Xml,
            FileType.Xml,
            FileType.Markdown,
            FileType.Html,

            FileType.Zip,
            FileType.Rar,
            FileType.Unknown
        };
    }

    public static IEnumerable<FileType> GetImageFileTypes(this FileType fileType)
    {
        return new[]
        {
            FileType.Jpg,
            FileType.Jpeg,
            FileType.Png,
            FileType.Gif
        };
    }

    public static IEnumerable<FileType> GetDocumentFileTypes(this FileType fileType)
    {
        return new[]
        {
            FileType.Pdf,
            FileType.Doc,
            FileType.Docx,
            FileType.Xls,
            FileType.Xlsx,
            FileType.Ppt,
            FileType.Pptx
        };
    }
}