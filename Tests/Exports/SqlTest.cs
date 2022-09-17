using SmartCrawler.Exports;

namespace Tests.Exports;

class TestSubItem
{
    public int sub1 { get; }
    public string? optionalFunctional { get; init; }
    public string? optionalEmpty { get; init; }

    public TestSubItem(int sub1)
    {
        this.sub1 = sub1;
    }
}

class TestItem
{
    public string item { get; }
    public string? optionalItem { get; init; }
    public TestSubItem subItem { get; }
    public TestSubItem subOptional { get; init; }
    public TestSubItem subOptionalEmpty { get; init; }

    public TestItem(TestSubItem subItem, string a)
    {
        this.subItem = subItem;
        item = a;
    }
}

public class SqlTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void BuildTableTest()
    {
        string expectedTableScript = $"CREATE TABLE CRAWLED_DATA (\nitem TEXT,\noptionalItem TEXT,\nsubItem_sub1 INT,\nsubItem_optionalFunctional TEXT,\nsubOptional_sub1 INT,\nsubOptional_optionalFunctional TEXT);";
        TestSubItem subItem = new TestSubItem(12){optionalFunctional = "hello2"};
        TestItem item = new TestItem(subItem, "ahoj"){subOptional = subItem, optionalItem = "hello"};

        Sql<TestItem> sqlExport = new Sql<TestItem>(new ExportOptions());
        string result = sqlExport.BuildTable(item);
        Assert.That(result, Is.EqualTo(expectedTableScript));
    }
}