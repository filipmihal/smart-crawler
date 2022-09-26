namespace SmartCrawler.Exports;

public enum UrlExportSeparator
{
    Url,
    SingleFile
}

public struct ExportOptions
{
    public ExportOptions()
    {
    }

    public UrlExportSeparator Separator { get; init; } = UrlExportSeparator.SingleFile;
    public string Filename { get; init; } = "exported_data";
}