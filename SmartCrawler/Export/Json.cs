using System.Text.Json;
using System.Text.Json.Serialization;
using SmartCrawler.Modules;

namespace SmartCrawler.Export;

public static class Json
{
    public static string Export(List<DatasetItem> items)
    {
        JsonSerializerOptions options = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        string exportedJson = JsonSerializer.Serialize<List<DatasetItem>>(items, options);

        return exportedJson;
    }
}