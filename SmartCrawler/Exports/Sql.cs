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
public class Sql<T>: ExportBase<T>
{
    private static SupportedType[] SupportedTypes = new[]
    {
        new SupportedType(typeof(bool), "BOOL"),
        new SupportedType(typeof(string), "TEXT"),
        new SupportedType(typeof(int), "INT"),
        new SupportedType(typeof(decimal), "DECIMAL"),
        new SupportedType(typeof(float), "FLOAT"),
        new SupportedType(typeof(double), "DOUBLE"),
        new SupportedType(typeof(DateTime), "DATETIME"),
        new SupportedType(typeof(string[]), "TEXT"),
    };

    public static bool IsTypeSupported(Type type)
    {
        return  SupportedTypes.Any(supportedType => supportedType.Type == type);
    }
        
    public override string GetExtension()
    {
        return "sql";
    }

    private static string BuildTableRecursively(Type type, object sample, string prefix)
    {
        string tableSql = "";
        var properties = type.GetProperties();
        foreach (var property in properties)
        {
            var propertyValue = property.GetValue(sample);
            if (IsTypeSupported(property.PropertyType) && propertyValue is not null)
            {
                // TODO: convert arrays
                tableSql +=
                    $"\n{prefix}{property.Name} {Array.Find(SupportedTypes, (type) => type.Type == property.PropertyType).SqlEquivalent},";
            
            }
            else if (propertyValue is not null && !property.PropertyType.IsValueType)
            {
                string newPrefix = property.Name + "_";
                tableSql += $"{BuildTableRecursively(property.PropertyType, propertyValue, newPrefix)}";
            }
        }

        return tableSql;
    }

    public string BuildTable(T sample)
    {
        string tableSql = "CREATE TABLE CRAWLED_DATA (";
        Type dataset = typeof(T);
        var properties = dataset.GetProperties();
        if (sample != null) tableSql += BuildTableRecursively(dataset, sample, "");
        tableSql = tableSql.Substring(0,tableSql.Length-1);
        return tableSql + ");";
    }

    public override void Export(List<T> items)
    {
        if (items.Count == 0)
        {
            throw new NoDataForSqlExportException();
        }
        Console.WriteLine(BuildTable(items[0]));
    }
    
    public Sql(ExportOptions exportOptions) : base(exportOptions)
    {
        
    }
    
}

public class NoDataForSqlExportException : Exception{}
