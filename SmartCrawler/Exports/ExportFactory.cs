
namespace SmartCrawler.Exports;

public static class ExportFactory
{
    public static ExportBase GenerateExport(ExportType exportType, ExportOptions exportOptions)
    {
        switch (exportType)
        {
            case ExportType.Json:
                return new Json(exportOptions);
            default:
                throw new ExportTypeNotFoundException();
        }
    }
}

public class ExportTypeNotFoundException : Exception{}