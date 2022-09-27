using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartCrawler.Exports;

public class Json<T> : ExportBase<T>
{
    public override string GetExtension()
    {
        return "json";
    }

    public override void Export(List<T> items)
    {
        JsonSerializerOptions options = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        switch (ExportOptions.Separator)
        {
            case UrlExportSeparator.Url:
                {
                    for (int idx = 0; idx < items.Count; idx++)
                    {
                        string exportedJson = JsonSerializer.Serialize<T>(items[idx], options);
                        string digits = "D" + items.Count;
                        File.WriteAllText($@"{ExportOptions.Filename}_{idx.ToString(digits)}.{GetExtension()}", exportedJson);
                    }
                    break;
                }
            case UrlExportSeparator.SingleFile:
                {
                    string exportedJson = JsonSerializer.Serialize<List<T>>(items, options);
                    File.WriteAllText($@"{ExportOptions.Filename}.{GetExtension()}", exportedJson);
                    break;
                }
            default:
                throw new ExportSeparatorNotFoundException();
        }
    }

    public Json(ExportOptions exportOptions) : base(exportOptions)
    {

    }

}

public class ExportSeparatorNotFoundException : Exception { }