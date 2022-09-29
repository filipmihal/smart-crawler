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

    [Test]
    public void TestInvalidAttributeParsing()
    {
        string testText1 = "    @@@ href='http://google.com'";
        StringParser parser = new StringParser(testText1);
        HtmlAttributes attributes = Document.ParseAttributes(parser);
        Assert.That(attributes.GetAvailableAttributes().Count, Is.EqualTo(1));
        Assert.That(attributes.Get("href"), Is.EqualTo("http://google.com"));
    }

    [Test]
    public void TestRootSingleTags()
    {
        string singleTags = "<br/> <img src='game.jpg' />    ";
        Element[] elements = Document.ParseHtml(singleTags);
        if (elements[0] is not HtmlElement)
        {
            throw new Exception();
        }
        if (elements[1] is not HtmlElement)
        {
            throw new Exception();
        }
        Assert.That(elements.Length, Is.EqualTo(2));
        Assert.That(elements[0].Name, Is.EqualTo("br"));
        Assert.That(elements[1].Name, Is.EqualTo("img"));

    }


    [Test]
    public void TestRootSingleTagsInvalid()
    {
        string singleTags = "<br !# # # # /> <img src='game.jpg' $$ />  <video ^^  ";
        Element[] elements = Document.ParseHtml(singleTags);

        Assert.That(elements.Length, Is.EqualTo(2));
        if (elements[0] is not HtmlElement)
        {
            throw new Exception();
        }
        if (elements[1] is not HtmlElement)
        {
            throw new Exception();
        }
        Assert.That(elements[0].Name, Is.EqualTo("br"));
        Assert.That(elements[1].Name, Is.EqualTo("img"));
    }

    [Test]
    public void TestTextElement()
    {
        string singleTags = " <img src='game.jpg' $$ />  hello world <video/>  ";
        Element[] elements = Document.ParseHtml(singleTags);

        Assert.That(elements.Length, Is.EqualTo(3));
        if (elements[1] is not TextElement)
        {
            throw new Exception();
        }
        Assert.That(((TextElement)elements[1]).Content, Is.EqualTo("hello world "));
    }

    [Test]
    public void TestNestedHtml()
    {
        string validHtml = @"</random>
        <html>
            hello friends!
            <img/>
            <div>
                hello
            </div>
        </html>
         ";
        Element[] elements = Document.ParseHtml(validHtml);

        Assert.That(elements.Length, Is.EqualTo(1));
        if (elements[0] is not HtmlElement)
        {
            throw new Exception();
        }

        Assert.That(((HtmlElement)elements[0]).Children.Count, Is.EqualTo(3));
    }

    [Test]
    public void TestMismatchedNestedHtml()
    {
        string validHtml = @"
        <html>
            text
            <img/>
            <div>
                <a>
                text2
            </div>
        </html>
         ";
        Element[] elements = Document.ParseHtml(validHtml);
        Assert.That(((HtmlElement)elements[0]).Children.Count, Is.EqualTo(3));
        Assert.That(((HtmlElement)elements[0]).Children[2].Name, Is.EqualTo("div"));

        HtmlElement div = (HtmlElement)((HtmlElement)elements[0]).Children[2];
        Assert.That(div.Children.Count, Is.EqualTo(1));

    }

    [Test]
    public void TestElementRelationships()
    {
        string validHtml = @"
        <meta />
        <div>
        <h1>Hello</h1>
        <h2>Ahoj</h2>
        <h3>Ciao</h3>
        </div>
         ";
        Element[] elements = Document.ParseHtml(validHtml);
        HtmlElement div = (HtmlElement)elements[1];
        Assert.That(div.Children[0].Name, Is.EqualTo("h1"));
        Assert.That(div.Children[1].Name, Is.EqualTo("h2"));
        Assert.That(div.Children[2].Name, Is.EqualTo("h3"));

        Assert.That(div.Children[0].NextElement, Is.EqualTo(div.Children[1]));
        Assert.That(div.Children[1].NextElement, Is.EqualTo(div.Children[2]));
        Assert.That(div.Children[2].NextElement, Is.EqualTo(null));

        Assert.That(div.Children[0].PreviousElement, Is.EqualTo(null));
        Assert.That(div.Children[1].PreviousElement, Is.EqualTo(div.Children[0]));
        Assert.That(div.Children[2].PreviousElement, Is.EqualTo(div.Children[1]));

        Assert.That(div.Children[0].ParentElement, Is.EqualTo(div));
        Assert.That(((HtmlElement)div.Children[1]).Children[0].ParentElement, Is.EqualTo(div.Children[1]));

    }
}