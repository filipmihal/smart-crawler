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
    public HtmlElement? NextElement { get; set; }

    public HtmlElement? ParentElement { get; }


    public HtmlAttributes Attributes { get; }

    public HtmlElement(string name, HtmlAttributes attributes, HtmlElement? previousElement, HtmlElement? parentElement)
    {

        // name must be in lower case
        Name = name.ToLower();
        Attributes = attributes;
        PreviousElement = previousElement;
        ParentElement = parentElement;
        Children = new List<HtmlElement>();
    }

    /// <summary>
    /// Adds a child to the element
    /// </summary>
    /// <remarks>
    /// Add children in order they occur in DOM
    /// </remarks/>
    public void AddChild(HtmlElement element)
    {
        Children.Add(element);
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
