using SmartCrawler.Modules;

namespace SmartCrawler.Exports;

public struct SupportedType
{
    public SupportedType(Type type, string sqlEquivalent)
    {
        Type = type;
        SqlEquivalent = sqlEquivalent;
    }

    public Type Type;
    public string SqlEquivalent;
}
public class Sql: ExportBase
{
    private static SupportedType[] SupportedTypes = new[]
    {
        new SupportedType(typeof(bool), "BOOL"),
        new SupportedType(typeof(string), "TEXT"),
        new SupportedType(typeof(int), "INT"),
        new SupportedType(typeof(decimal), "DECIMAL"),
        new SupportedType(typeof(float), "FLOAT"),
        new SupportedType(typeof(double), "DOUBLE"),
        new SupportedType(typeof(DateTime), "DATETIME")
    };

    public static bool IsTypeSupported(Type type)
    {
        return  SupportedTypes.Any(supportedType => supportedType.Type == type);
    }
        
    public override string GetExtension()
    {
        return "sql";
    }

    private string BuildTable(DatasetItem sample)
    {
        string tableSql = "CREATE TABLE CRAWLED_DATA (";
        Type dataset = typeof(DatasetItem);
        var properties = dataset.GetProperties();
        foreach (var property in properties)
        {
            if (IsTypeSupported(property.PropertyType) && property.GetValue(sample) is not null)
            {
                tableSql +=
                    $"\n{property.Name} {Array.Find(SupportedTypes, (type) => type.Type == property.PropertyType).SqlEquivalent},";
            
            }
            // TODO: Add recursion
            // Console.WriteLine(property.GetValue(sample) is null);
        }

        return tableSql + ");";
    }

    public override void Export(List<DatasetItem> items)
    {
        if (items.Count == 0)
        {
            throw new NoDataForSQLExport();
        }
        Console.WriteLine(BuildTable(items[0]));
    }
    
    public Sql(ExportOptions exportOptions) : base(exportOptions)
    {
        
    }
    
}

public class NoDataForSQLExport : Exception{}
