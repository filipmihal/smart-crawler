namespace SmartCrawler.Parser;

public class Element
{
    /// <summary>
    /// Html tag name in lower case
    /// </summary>
    /// <remarks>
    /// For TextElement the name is equal to 'text'
    /// </remarks/>
    public string Name { get; }

    /// <summary>
    /// The previous node that comes before the current one in the DOM hierarchy
    /// </summary>
    /// <remarks>
    /// HtmlElement type is used by default
    /// </remarks/>
    public Element? PreviousElement { get; private set; }

    /// <summary>
    /// The next node that comes after the current one in the DOM hierarchy
    /// </summary>
    /// <remarks>
    /// HtmlElement type is used by default
    /// </remarks/>
    public Element? NextElement { get; set; }

    public HtmlElement? ParentElement { get; }


    public Element(string name, Element? previousElement, HtmlElement? parentElement)
    {
        // name must be in lower case
        Name = name.ToLower();
        PreviousElement = previousElement;
        ParentElement = parentElement;
    }

}
