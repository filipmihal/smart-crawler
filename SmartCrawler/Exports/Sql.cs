namespace SmartCrawler.Exports;

public enum SqlCrawlType
{
    NewTable,
    Columns,
    Values

}
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
public class Sql<T> : ExportBase<T>
{
    private static string tableName = "CRAWLED_DATA";

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


    /// <summary>
    ///  Converts property value to an sql format
    /// </summary>
    /// <remarks>
    /// Arrays are converted to a string and the values are separated by a comma
    /// ["hello", "coffee"] ==> TEXT 'hello, coffee'
    /// </remarks>
    private static string ConvertValue(SupportedType type, object propValue)
    {
        if (type.Type == typeof(string[]))
        {
            string[]? newVal = (string[])propValue!;
            return "'" + string.Join(",", newVal) + "'";
        }

        string? result = propValue.ToString();

        if (result is null)
        {
            throw new SqlExportMismatchException();
        }

        if (type.Type == typeof(string))
        {
            return "'" + result + "'";
        }

        return result;
    }

    public static bool IsTypeSupported(Type type)
    {
        return SupportedTypes.Any(supportedType => supportedType.Type == type);
    }

    public override string GetExtension()
    {
        return "sql";
    }

    /// <summary>
    /// todo
    /// </summary>
    /// <param name="type">todo</param>
    private static List<string> CrawlTypeRecursively(Type type, object sample, string prefix, SqlCrawlType crawlType)
    {
        List<string> sql = new List<string>();
        var properties = type.GetProperties();
        foreach (var property in properties)
        {
            var propertyValue = property.GetValue(sample);
            if (IsTypeSupported(property.PropertyType) && propertyValue is not null)
            {
                SupportedType supportedType =
                    Array.Find(SupportedTypes, (supType) => supType.Type == property.PropertyType);
                switch (crawlType)
                {
                    case SqlCrawlType.NewTable:
                        sql.Add($"{prefix}{property.Name} {supportedType.SqlEquivalent}");
                        break;
                    case SqlCrawlType.Values:
                        string convertedValue = ConvertValue(supportedType, propertyValue);
                        sql.Add(convertedValue);
                        break;
                    case SqlCrawlType.Columns:
                        sql.Add($"{prefix}{property.Name}");
                        break;
                }
            }
            else if (propertyValue is not null && !property.PropertyType.IsValueType)
            {
                string newPrefix = property.Name + "_";
                sql.AddRange(CrawlTypeRecursively(property.PropertyType, propertyValue, newPrefix, crawlType));
            }
        }

        return sql;
    }

    public string BuildInsert(T sample)
    {
        Type dataset = typeof(T);
        string tableSql = "INSERT INTO CRAWLED_DATA (";
        List<string> data = CrawlTypeRecursively(dataset, sample, "", SqlCrawlType.Columns);

        return tableSql + string.Join(',', data) + ")";
    }

    public string BuildTable(T sample)
    {
        string tableSql = "CREATE TABLE CRAWLED_DATA (";
        Type dataset = typeof(T);
        var properties = dataset.GetProperties();
        if (sample is not null)
        {
            var columns = CrawlTypeRecursively(dataset, sample, "", SqlCrawlType.NewTable);
            tableSql += string.Join("\n,", columns);
        }
        return tableSql + ");";
    }

    public string BuildRows(List<T> dataset)
    {
        string finalString = "VALUES\n";
        Type datasetType = typeof(T);
        List<string> rows = new List<string>();
        foreach (var item in dataset)
        {
            var values = CrawlTypeRecursively(datasetType, item, "", SqlCrawlType.Values);
            rows.Add("(" + string.Join(",", values) + ")");
        }

        finalString += string.Join(",\n", rows);

        return finalString;
    }


    public override void Export(List<T> items)
    {
        if (items.Count == 0)
        {
            throw new NoDataForSqlExportException();
        }

        if (ExportOptions.Separator == UrlExportSeparator.Url)
        {
            throw new SqlUnsupportedSeparatorException();
        }

        string tableConstruction = BuildTable(items[0]);
        string insert = BuildInsert(items[0]);
        string rows = BuildRows(items);

        string finalSql = tableConstruction + "\n\n" + insert + "\n\n" + rows + ";";

        File.WriteAllText($@"{ExportOptions.Filename}.{GetExtension()}", finalSql);


    }

    public Sql(ExportOptions exportOptions) : base(exportOptions)
    {

    }

}

public class NoDataForSqlExportException : Exception { }
public class SqlExportMismatchException : Exception { }
public class SqlUnsupportedSeparatorException : Exception { }