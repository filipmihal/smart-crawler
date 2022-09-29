namespace SmartCrawler.Parser;

public class TextElement : Element
{
    public string Content { get; }
    public TextElement(string content, Element? previousElement, HtmlElement? parentElement) : base("text", previousElement, parentElement)
    {
        Content = content;
    }
}