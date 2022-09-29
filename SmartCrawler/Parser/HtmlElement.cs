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

    /// <summary>
    /// Finds the first tag matching the name in the list of children
    /// </summary>
    /// <remarks>
    /// returns null if there is no such a tag
    /// </remarks/>
    public Element? QueryChildren(string tag)
    {
        return Children.Find(elem => elem.Name == tag);
    }

    private Element? QueryRecursive(string tag, List<Element> children, ref int left)
    {
        foreach (Element child in children)
        {
            if (child.Name == tag)
            {
                if (left == 1)
                {
                    return child;
                }
                left--;
            }
            if (child is HtmlElement)
            {
                Element? elem = QueryRecursive(tag, ((HtmlElement)child).Children, ref left);
                if (elem is not null)
                {
                    if (left == 1)
                    {
                        return elem;
                    }
                    left--;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Finds the first tag matching the name in the content of this element
    /// </summary>
    /// <remarks>
    /// returns null if there is no such a tag
    /// uses DFS to find the element
    /// </remarks/>
    public Element? Query(string tag)
    {
        int a = 1;
        return QueryRecursive(tag, Children, ref a);
    }


    /// <summary>
    /// Finds the nth element that matches the tag name
    /// </summary>
    /// <summary>
    /// uses DFS to find the element
    /// </summary>
    public Element? Query(string tag, int nth)
    {
        if (nth < 1)
        {
            throw new OnlyNaturalNumbersException();
        }
        return QueryRecursive(tag, Children, ref nth);
    }

    private List<Element> QueryAllRecursive(string tag, List<Element> children)
    {
        List<Element> elements = new List<Element>();
        foreach (Element child in children)
        {
            if (child.Name == tag)
            {
                elements.Add(child);
            }
            if (child is HtmlElement)
            {
                List<Element> recursiveElements = QueryAllRecursive(tag, ((HtmlElement)child).Children);
                elements.AddRange(recursiveElements);
            }
        }

        return elements;
    }

    /// <summary>
    /// Finds all tags matching the name in all children nodes
    /// </summary>
    /// <summary>
    /// The returned order is done by a DFS algorithm
    /// </summary>
    public Element[] QueryAll(string tag)
    {
        return QueryAllRecursive(tag, Children).ToArray();
    }
}

class OnlyNaturalNumbersException : Exception { }
