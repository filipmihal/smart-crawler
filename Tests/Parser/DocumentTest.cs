using SmartCrawler.Parser;

namespace Tests.Parser;

public class DocumentTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestAttributesParser()
    {
        string testText1 = "href='http://google.com' control hello=\"hey\"/> nope";
        StringParser parser = new StringParser(testText1);
        HtmlAttributes attributes = Document.ParseAttributes(parser);
        Assert.That(attributes.GetAvailableAttributes().Count, Is.EqualTo(3));
        Assert.That(attributes.Get("href"), Is.EqualTo("http://google.com"));
        Assert.That(attributes.Get("control"), Is.EqualTo(""));
        Assert.That(attributes.Get("hello"), Is.EqualTo("hey"));
    }
}