namespace SmartCrawler.Parser;

public class HtmlElement : GeneralHtmlElement
{
    public HtmlAttributes Attributes = new HtmlAttributes();
    public HtmlElement(string name) : base(name)
    {
    }
}