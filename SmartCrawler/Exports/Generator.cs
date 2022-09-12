using Type = System.Type;

namespace SmartCrawler.Exports;

public static class Generator
{
    public static Base MatchExport(Type exportType, Options options)
    {
        switch (exportType)
        {
            case Type.Json:
                return new Json(options);
            default:
                throw new ExportTypeNotFoundException();
        }
    }
}

public class ExportTypeNotFoundException : Exception{}