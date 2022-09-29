using System.Text.RegularExpressions;

namespace SmartCrawler.Parser;

public class Document
{
    public static Regex HtmlTagRegex = new Regex("[0-9]|[a-z]|[A-Z]");

    public static HtmlAttributes ParseAttributes(StringParser parser)
    {
        HtmlAttributes attributes = new HtmlAttributes();
        Regex attributeNameRegex = new Regex(@"[0-9]|[a-z]|[A-Z]|_|-");
        while ( !parser.IsEndOfString && parser.Peek() != '>' && parser.Peek() != '/')
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
    public static HtmlElement[] ParseHtml(string text)
    {
        if (text.Length == 0)
        {
            return Array.Empty<HtmlElement>();
        }
        List<HtmlElement> rootElements = new List<HtmlElement>();
        StringParser parser = new StringParser(text);
        HtmlElement? parentElem = null;
        HtmlElement? previousElem = null;

        while (!parser.IsEndOfString)
        {
            parser.SkipAll(' ');
            if (parser.Peek() == '<')
            {
                parser.Next();
                if (parser.Peek() == '/')
                {
                    // closing tag

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
                    if (parser.Peek() == '/')
                    {
                        parser.SkipUntil('>');
                        HtmlElement newSingleTagElem = new HtmlElement(tagName, attributes, previousElem, parentElem);
                        if (parentElem is not null)
                        {
                            parentElem.AddChild(newSingleTagElem);
                        }
                        else
                        {
                            rootElements.Add(newSingleTagElem);
                        }
                        if (previousElem is not null)
                        {
                            previousElem.NextElement = newSingleTagElem;
                        }

                        parser.Next();
                    }
                }
            }
            else
            {
                parser.Next();
                // TODO: process content
            }
        }
        return rootElements.ToArray();
    }
}