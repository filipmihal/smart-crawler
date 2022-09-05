namespace Tests;

public class LinkParserTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ParseValidHtml()
    {
        List<string> expected = new List<string>(new string[] { "https://www.w3.org/", "https://www.google.com/" });
        string validHtml = @"<h2>Absolute URLs</h2>
            <p><a href=""https://www.w3.org/"">W3C</a></p>
            <p><a href='https://www.google.com/'>Google</a></p>";
        var actual = SmartCrawler.CrawlerHelpers.ParseLinks(validHtml, "https://www.w3.org");
        CollectionAssert.AreEqual(expected, actual);
    }
    
    [Test]
    public void ParseInvalidHtml()
    {
        List<string> expected = new List<string>(new string[] { "https://www.w3.org/" });
        string validHtml = @"<h1>Absolute URLs
            <p>nn    <a href=""https://www.w3.org/"">W3C</p>
            <p><a href='https://www.google.com/>Google</a></p>";
        var actual = SmartCrawler.CrawlerHelpers.ParseLinks(validHtml, "https://www.w3.org");
        CollectionAssert.AreEqual(expected, actual);
    }
    
    [Test]
    public void RelativeUrls()
    {
        List<string> expected = new List<string>(new string[] { "https://www.w3.org/ahoj/html_images.asp", "https://www.w3.org/css/default.asp" });
        string validHtml = @"<h2>Relative URLs</h2>
            <p><a href=""html_images.asp"">HTML Images</a></p>
            <p><a href='/css/default.asp'>CSS Tutorial</a></p>";
        var actual = SmartCrawler.CrawlerHelpers.ParseLinks(validHtml, "https://www.w3.org/ahoj");
        CollectionAssert.AreEqual(expected, actual);
    }
    
    [Test]
    public void RelativeUrlsComplexParent()
    {
        List<string> expected = new List<string>(new string[] { "https://www.w3.org/ahoj/hello/html_images.asp", "https://www.w3.org/css/default.asp" });
        string validHtml = @"<h2>Relative URLs</h2>
            <p><a href=""html_images.asp"">HTML Images</a></p>
            <p><a href='/css/default.asp'>CSS Tutorial</a></p>";
        var actual = SmartCrawler.CrawlerHelpers.ParseLinks(validHtml, "https://www.w3.org/ahoj/hello/?val=true");
        CollectionAssert.AreEqual(expected, actual);
    }
}