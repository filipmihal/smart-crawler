using System.Text.RegularExpressions;

namespace SmartCrawler.Parser;

public class Document
{
    public static Regex HtmlTagRegex = new Regex("[0-9]|[a-z]|[A-Z]");

    public static HtmlAttributes ParseAttributes(StringParser parser)
    {
        HtmlAttributes attributes = new HtmlAttributes();
        Regex attributeNameRegex = new Regex(@"[0-9]|[a-z]|[A-Z]|_|-");
        while (!parser.IsEndOfString && parser.Peek() != '>' && parser.Peek() != '/')
        {
            parser.SkipAll(new Regex(@"[\s]"));
            string attributeName = parser.ReadAll(attributeNameRegex);
            if (attributeName.Length == 0)
            {
                if (parser.Peek() != '>' && parser.Peek() != '/')
                {
                    parser.Next();
                }
                continue;
            }
            if (parser.Peek() == ' ' || parser.Peek() == '>' || parser.Peek() == '/')
            {
                attributes.Add(attributeName, "");
                continue;
            }
            if (parser.Peek() == '=')
            {
                parser.Next();
                if (parser.Peek() == '"')
                {
                    parser.Next();
                    string attributeValue = parser.ReadUntil(new Regex("[\"]"));
                    attributes.Add(attributeName, attributeValue);
                    parser.Next();
                }
                else if (parser.Peek() == '\'')
                {
                    parser.Next();
                    string attributeValue = parser.ReadUntil(new Regex("[\']"));
                    attributes.Add(attributeName, attributeValue);
                    parser.Next();
                }
            }
        }

        return attributes;
    }
    public static Element[] ParseHtml(string text)
    {
        if (text.Length == 0)
        {
            return Array.Empty<Element>();
        }
        List<Element> rootElements = new List<Element>();
        StringParser parser = new StringParser(text);
        HtmlElement? parentElem = null;
        HtmlElement? previousElem = null;

        Action<Element> ConnectElements =
       (Element elem) =>
       {
           if (parentElem is not null)
           {
               parentElem.AddChild(elem);
           }
           else
           {
               rootElements.Add(elem);
           }
           if (previousElem is not null)
           {
               previousElem.NextElement = elem;
           }

       };


        while (!parser.IsEndOfString)
        {
            parser.SkipAll(new Regex(@"[\s]|[\n]"));
            if (parser.Peek() == '<')
            {
                parser.Next();
                if (parser.Peek() == '/')
                {
                    // closing tag
                    string tagName = parser.ReadAll(HtmlTagRegex).ToLower();
                    if (parentElem is null)
                    {
                        parser.SkipUntil('>');
                        parser.Next();
                        continue;
                    }
                    if (parentElem.Name != tagName)
                    {
                        // Html mismatch
                        // TODO: go through parents and close all of them until you get to the matching tag.
                        // if you can't get there, then close the current one
                    }
                    parser.SkipUntil('>');
                    parser.Next();
                    parentElem = parentElem.ParentElement;
                }
                else
                {
                    // opening tag
                    string tagName = parser.ReadAll(HtmlTagRegex).ToLower();

                    // The tag name contains invalid characters
                    if (tagName.Length == 0)
                    {
                        continue;
                    }

                    HtmlAttributes attributes = ParseAttributes(parser);
                    parser.SkipAll(' ');
                    // Check if tag is a single tag
                    if (parser.Peek() == '/')
                    {
                        parser.SkipUntil('>');
                        HtmlElement newSingleTagElem = new HtmlElement(tagName, attributes, previousElem, parentElem);
                        ConnectElements(newSingleTagElem);
                        previousElem = newSingleTagElem;
                    }
                    else if (parser.Peek() == '>')
                    {
                        HtmlElement newPairedTag = new HtmlElement(tagName, attributes, previousElem, parentElem);
                        ConnectElements(newPairedTag);
                        parentElem = newPairedTag;
                        previousElem = null;
                    }
                    parser.Next();
                }
            }
            else if (parser.Peek() != ' ')
            {
                string content = parser.ReadUntil(new Regex("<"));
                TextElement textElem = new TextElement(content, parentElem, previousElem);
                ConnectElements(textElem);

            }
        }
        return rootElements.ToArray();
    }
}