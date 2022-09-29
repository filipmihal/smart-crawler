using SmartCrawler.Parser;

namespace Tests.Parser;

public class HtmlElementTest
{
    private string html = @"
    <html>
    <div>
        <div>
            <h2>1</h2>
        </div>
        <section>
            <H2>2</H2>
        </section>
        <H2>3</H2>
    </div>
    <H2>4</H2>
    </html>
    ";

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestQueryAll()
    {
        Element[] elements = Document.ParseHtml(html);
        Element[] queriedHeadlines = ((HtmlElement)elements[0]).QueryAll("h2");

        Assert.That(queriedHeadlines.Length, Is.EqualTo(4));
        for (int i = 0; i < 4; i++)
        {
            HtmlElement h2 = (HtmlElement)queriedHeadlines[i];
            TextElement text = (TextElement)h2.Children[0];
            Assert.That(text.Content, Is.EqualTo((i + 1).ToString()));
        }

    }


    [Test]
    public void TestQuery()
    {
        Element[] elements = Document.ParseHtml(html);
        Element? elem = ((HtmlElement)elements[0]).Query("h2");

        TextElement? textElem = (elem as HtmlElement).Children[0] as TextElement;

        Assert.That(textElem.Content, Is.EqualTo("1"));
    }

    [Test]
    public void TestQueryNotFound()
    {
        Element[] elements = Document.ParseHtml(html);
        Element? elem = ((HtmlElement)elements[0]).Query("h3");

        Assert.That(elem, Is.EqualTo(null));
    }

    [Test]
    public void TestQueryNth()
    {
        Element[] elements = Document.ParseHtml(html);
        Element? elem = ((HtmlElement)elements[0]).Query("h2", 2);

        TextElement? textElem = (elem as HtmlElement).Children[0] as TextElement;

        Assert.That(textElem.Content, Is.EqualTo("2"));
    }
    
    [Test]
    public void TestQueryNthNotFound()
    {
        Element[] elements = Document.ParseHtml(html);
        Element? elem = ((HtmlElement)elements[0]).Query("h2", 100);
        
        Assert.That(elem, Is.EqualTo(null));
    }
}
