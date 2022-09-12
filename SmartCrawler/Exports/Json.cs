using System.Text.Json;
using System.Text.Json.Serialization;
using SmartCrawler.Modules;

namespace SmartCrawler.Exports;

public class Json: Base
{
    public override void Export(List<DatasetItem> items)
    {
        JsonSerializerOptions options = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        string exportedJson = JsonSerializer.Serialize<List<DatasetItem>>(items, options);
    }
    
    public Json(Options options) : base(options)
    {
        
    }
    
}