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
        string expectedTableScript = $"CREATE TABLE CRAWLED_DATA (item TEXT\n,optionalItem TEXT\n,subItem_sub1 INT\n,subItem_optionalFunctional TEXT\n,subOptional_sub1 INT\n,subOptional_optionalFunctional TEXT);";
        TestSubItem subItem = new TestSubItem(12) { optionalFunctional = "hello2" };
        TestItem item = new TestItem(subItem, "ahoj") { subOptional = subItem, optionalItem = "hello" };

        Sql<TestItem> sqlExport = new Sql<TestItem>(new ExportOptions());
        string result = sqlExport.BuildTable(item);
        Assert.That(result, Is.EqualTo(expectedTableScript));
    }

    [Test]
    public void BuildInsertTest()
    {
        string expectedInsertScript = $"INSERT INTO CRAWLED_DATA (item,optionalItem,subItem_sub1,subItem_optionalFunctional,subOptional_sub1,subOptional_optionalFunctional)";
        TestSubItem subItem = new TestSubItem(12) { optionalFunctional = "hello2" };
        TestItem item = new TestItem(subItem, "ahoj") { subOptional = subItem, optionalItem = "hello" };

        Sql<TestItem> sqlExport = new Sql<TestItem>(new ExportOptions());
        string result = sqlExport.BuildInsert(item);
        Assert.That(result, Is.EqualTo(expectedInsertScript));
    }

    [Test]
    public void BuildRowsTest()
    {
        string expectedInsertScript = $"VALUES\n('ahoj','hello',12,'hello2',12,'hello2'),\n('ahoj','hello',12,'hello2',12,'hello2')";
        TestSubItem subItem = new TestSubItem(12) { optionalFunctional = "hello2" };
        TestItem item = new TestItem(subItem, "ahoj") { subOptional = subItem, optionalItem = "hello" };
        List<TestItem> dataset = new List<TestItem>() { item };
        dataset.Add(item);

        Sql<TestItem> sqlExport = new Sql<TestItem>(new ExportOptions());
        string result = sqlExport.BuildRows(dataset);
        Assert.That(result, Is.EqualTo(expectedInsertScript));
    }
}