namespace SmartCrawler.Exports;

public enum UrlExportSeparator
{
    Domain,
    Url,
    SingleFile
}

public struct Options
{
    public Options()
    {
    }

    public UrlExportSeparator Separator { get; init; } = UrlExportSeparator.SingleFile;
    public string Filename { get; init; } = "exportedData";
}