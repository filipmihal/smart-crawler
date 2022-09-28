namespace SmartCrawler.Parser;

public class HtmlElement
{
    /// <summary>
    /// Html tag name in lower case
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// List of children
    /// The order is equivalent to DOM hierarchy
    /// </summary>
    /// <remarks>
    /// HtmlElement type is used by default
    /// </remarks/>
    public List<HtmlElement> Children { get; private set; }

    /// <summary>
    /// The previous node that comes before the current one in the DOM hierarchy
    /// </summary>
    /// <remarks>
    /// HtmlElement type is used by default
    /// </remarks/>
    public HtmlElement? PreviousElement { get; private set; }

    /// <summary>
    /// The next node that comes after the current one in the DOM hierarchy
    /// </summary>
    /// <remarks>
    /// HtmlElement type is used by default
    /// </remarks/>
    public HtmlElement? NextElement { get; private set; }

    public HtmlAttributes Attributes = new HtmlAttributes();

    public HtmlElement(string name)
    {
        Name = name;
    }

    public HtmlElement Query(string tag)
    {
        throw new Exception();
    }

    public HtmlElement Query(string tag, int index)
    {
        throw new Exception();
    }

    public HtmlElement[] QueryAll(string tag, int index)
    {
        throw new Exception();
    }

}
