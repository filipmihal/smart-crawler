namespace SmartCrawler.Exports
{
    public enum CSVCrawlerType
    {
        Values,
        Header
    }
    public class Csv<T> : ExportBase<T>
    {
        private static Type[] _supported = new[] { typeof(bool), typeof(string), typeof(int), typeof(decimal), typeof(float), typeof(double), typeof(DateTime), typeof(string[]), typeof(int[]) };


        public Csv(ExportOptions exportOptions) : base(exportOptions)
        {
        }

        public static bool IsTypeSupported(Type type)
        {
            return _supported.Any(supportedType => supportedType == type);
        }

        public static string ConvertEnumerableToValue(Array? items)
        {
            if (items.Length == 0)
            {
                return "";
            }
            string[]? newVal = (string[])items!;
            return string.Join(';', newVal);
        }

        private static List<string> CrawlTypeRecursively(Type type, object sample, string prefix, CSVCrawlerType crawlType)
        {
            List<string> csvArray = new List<string>();
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(sample);
                if (IsTypeSupported(property.PropertyType) && propertyValue is not null)
                {
                    switch (crawlType)
                    {
                        case CSVCrawlerType.Values:
                            if (propertyValue is null)
                            {
                                throw new CanConvertToStringException();
                            }
                            if (property.PropertyType == typeof(string[]) || property.PropertyType == typeof(int[]))
                            {
                                csvArray.Add($"\"{ConvertEnumerableToValue(propertyValue as Array)}\"");
                                break;
                            }
                            csvArray.Add($"\"{propertyValue}\"");
                            break;
                        case CSVCrawlerType.Header:
                            csvArray.Add($"\"{prefix}{property.Name}\"");
                            break;
                        default:
                            throw new CSVCrawlTypeNotSupportedException();

                    }
                }
                else if (propertyValue is not null && !property.PropertyType.IsValueType)
                {
                    string newPrefix = property.Name + "_";
                    csvArray.AddRange(CrawlTypeRecursively(property.PropertyType, propertyValue, newPrefix, crawlType));
                }
            }

            return csvArray;
        }

        public string BuildHeader(T sample)
        {
            var headerValues = CrawlTypeRecursively(typeof(T), sample, "", CSVCrawlerType.Header);
            return string.Join(',', headerValues);
        }

        public string BuildValues(List<T> items)
        {
            List<string> rows = new List<string>();
            items.ForEach(item => rows.Add(BuildValue(item)));
            return string.Join('\n', rows);
        }

        public string BuildValue(T item)
        {
            var crawled = CrawlTypeRecursively(typeof(T), item, "", CSVCrawlerType.Values);
            return string.Join(',', crawled);
        }

        public override string GetExtension()
        {
            return "csv";
        }

        public override void Export(List<T> items)
        {
            if (items.Count == 0)
            {
                throw new NoDataForCSVExportException();
            }
            string header = BuildHeader(items[0]);
            switch (ExportOptions.Separator)
            {
                case UrlExportSeparator.SingleFile:
                    {
                        string values = BuildValues(items);
                        string finalCsv = header + "\n" + values;
                        File.WriteAllText($@"{ExportOptions.Filename}.{GetExtension()}", finalCsv);
                        break;
                    }
                case UrlExportSeparator.Url:
                    {
                        for (int idx = 0; idx < items.Count; idx++)
                        {
                            string digits = "D" + Math.Ceiling(Math.Log10(items.Count + 1));
                            string row = BuildValue(items[idx]);
                            string finalCsv = header + "\n" + row;
                            File.WriteAllText($@"{ExportOptions.Filename}_{idx.ToString(digits)}.{GetExtension()}", finalCsv);
                        }
                        break;
                    }
                default:
                    throw new UrlExportSeparatorNotSupportedException();
            }
        }
    }

    class CanConvertToStringException : Exception { }
    class CSVCrawlTypeNotSupportedException : Exception { }
    public class NoDataForCSVExportException : Exception { }

}