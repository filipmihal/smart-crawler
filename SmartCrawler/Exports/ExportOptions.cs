namespace SmartCrawler.Exports;

public enum UrlExportSeparator
{
    Url,
    SingleFile,
    Domain
    // todo
}

public struct ExportOptions
{
    public ExportOptions()
    {
    }

    /// <summary>
    /// Defines the file format of the exported data
    /// </summary>
    /// <remarks>
    /// Files can be separated by URL (Each URL data are in a single file)
    /// All data can be in a single file
    /// Files can be grouped by domain
    /// </remarks>
    public UrlExportSeparator Separator { get; init; } = UrlExportSeparator.SingleFile;
    /// <summary>
    /// Filename of the exported data
    /// </summary>
    /// <remarks>
    /// when a Url separator is selected it serves as a prefix
    /// </remarks>
    public string Filename { get; init; } = "exported_data";
}