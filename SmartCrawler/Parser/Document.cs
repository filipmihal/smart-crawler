using System.Text.RegularExpressions;

namespace SmartCrawler.Parser;

public class Document
{
    public static Regex HtmlTagRegex = new Regex("[0-9]|[a-z]|[A-Z]");

    public static HtmlAttributes ParseAttributes(StringParser parser)
    {
        HtmlAttributes attributes = new HtmlAttributes();
        Regex attributeNameRegex = new Regex(@"[0-9]|[a-z]|[A-Z]|_|-");
        while (parser.Peek() != '>' && parser.Peek() != '/' && !parser.IsEndOfString)
        {
            parser.SkipAll(new Regex(@"[\s]"));
            string attributeName = parser.ReadAll(attributeNameRegex);
            if (attributeName.Length == 0)
            {
                parser.Next();
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
                if (parser.Peek() == '"' || parser.Peek() == '\'')
                {
                    parser.Next();
                    string attributeValue = parser.ReadUntil(new Regex("[\"]|[']"));
                    attributes.Add(attributeName, attributeValue);
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
        while (!parser.IsEndOfString)
        {
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
                    string tagName = parser.ReadAll(HtmlTagRegex);

                    // The tag name contains invalid characters
                    if (tagName.Length == 0)
                    {
                        continue;
                    }

                }
            }
        }
        throw new Exception();

    }
}