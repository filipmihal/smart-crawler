using SmartCrawler;

namespace Tests;

public class LinkFilteringTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void FilterCrossDomain()
    {
        List<string> testList = new List<string>(new[]
            { "https://www.w3.org/ahoj/cau", "https://www.q4.org/ahoj/cau", "https://www.w99.org/ahoj/cau" });
        List<CrawledSite> expected = new List<CrawledSite>(new []{ new CrawledSite("https://www.w3.org/ahoj/cau", 0) });
        List<CrawledSite> actual = SmartCrawler.SmartCrawler.FilterAndFormatUrls(testList, 0, false, "https://www.w3.org/");
        CollectionAssert.AreEqual(expected, actual);
    }
}