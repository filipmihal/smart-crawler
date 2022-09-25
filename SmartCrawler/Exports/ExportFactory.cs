
using SmartCrawler.Modules;

namespace SmartCrawler.Exports;

public static class ExportFactory<T>
{
    public static ExportBase<T> GenerateExport(ExportType exportType, ExportOptions exportOptions)
    {
        switch (exportType)
        {
            case ExportType.Json:
                return new Json<T>(exportOptions);
            case ExportType.Sql:
                return new Sql<T>(exportOptions);
            default:
                throw new ExportTypeNotFoundException();
        }
    }
}

public class ExportTypeNotFoundException : Exception{}