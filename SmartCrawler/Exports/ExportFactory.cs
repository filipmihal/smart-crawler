
using SmartCrawler.Modules;

namespace SmartCrawler.Exports;

public static class ExportFactory
{
    public static ExportBase<DatasetItem> GenerateExport(ExportType exportType, ExportOptions exportOptions)
    {
        switch (exportType)
        {
            case ExportType.Json:
                return new Json<DatasetItem>(exportOptions);
            case ExportType.Sql:
                return new Sql<DatasetItem>(exportOptions);
            default:
                throw new ExportTypeNotFoundException();
        }
    }
}

public class ExportTypeNotFoundException : Exception{}