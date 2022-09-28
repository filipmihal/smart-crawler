using SmartCrawler.Exports;

namespace Tests.Exports;

class TestCsvItem
{
    public decimal DecimalProp { get; init; }
    public string? StringProp { get; init; }
    public string? EmptyProp { get; init; }

    public SubClass? ClassProp { get; init; }
}

class SubClass
{
    public string? Item { get; init; }
}

public class CsvTest
{
    [SetUp]
    public void Setup()
    {
    }



    [Test]
    public void CSVBuildHeaderTest()
    {
        string expectedHeader = $"\"DecimalProp\",\"StringProp\",\"ClassProp_Item\"";
        SubClass cls = new SubClass() { Item = "item" };
        TestCsvItem item = new TestCsvItem() { DecimalProp = 30, ClassProp = cls, StringProp = "stringprop" };
        Csv<TestCsvItem> csvExport = new Csv<TestCsvItem>(new ExportOptions());
        string result = csvExport.BuildHeader(item);
        Assert.That(result, Is.EqualTo(expectedHeader));
    }
    
    [Test]
    public void CSVBuildValuesTest()
    {
        string expectedHeader = $"\"30\",\"stringprop\",\"item\"";
        SubClass cls = new SubClass() { Item = "item" };
        TestCsvItem item = new TestCsvItem() { DecimalProp = 30, ClassProp = cls, StringProp = "stringprop" };
        Csv<TestCsvItem> csvExport = new Csv<TestCsvItem>(new ExportOptions());
        string result = csvExport.BuildValue(item);
        Assert.That(result, Is.EqualTo(expectedHeader));
    }

}