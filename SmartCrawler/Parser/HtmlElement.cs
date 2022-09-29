namespace SmartCrawler.Parser;

public class HtmlElement : Element
{

    /// <summary>
    /// List of children
    /// The order is equivalent to DOM hierarchy
    /// </summary>
    /// <remarks>
    /// HtmlElement type is used by default
    /// </remarks/>
    public List<Element> Children { get; private set; }


    public HtmlAttributes Attributes { get; }

    public HtmlElement(string name, HtmlAttributes attributes, Element? previousElement, HtmlElement? parentElement) : base(name, previousElement, parentElement)
    {

        Attributes = attributes;
        Children = new List<Element>();
    }

    /// <summary>
    /// Adds a child to the element
    /// </summary>
    /// <remarks>
    /// Add children in order they occur in DOM
    /// </remarks/>
    public void AddChild(Element element)
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
