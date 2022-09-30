using SmartCrawler.Parser;
using NTextCat;
namespace SmartCrawler.Modules.LanguageDetection;

public class LanguageDetectionModule : IBaseModuleSetup<LanguageDetectionDataset>
{
    private string ParseTextFromElements(Element[] elements)
    {
        string final = "";
        foreach (var elem in elements)
        {
            if (elem is TextElement)
            {
                final += "\n" + ((TextElement)elem).Content;
            }
            else
            {
                if (elem is HtmlElement)
                {
                    List<Element> main = ((HtmlElement)elem).Children;
                    if (main.Count > 0)
                    {
                        if (main[0] is TextElement)
                        {
                            final += "\n" + ((TextElement)main[0]).Content;
                        }
                    }
                }
            }

        }
        return final;
    }
    public LanguageDetectionDataset Process(string html)
    {
        Element[] elements = Document.ParseHtml(html);
        foreach (var element in elements)
        {
            if (element.Name == "html")
            {
                string final = "";
                Element[] h1s = ((HtmlElement)element).QueryAll("h1");
                Element[] h2s = ((HtmlElement)element).QueryAll("h2");

                final += ParseTextFromElements(h1s);
                final += ParseTextFromElements(h2s);

                var factory = new RankedLanguageIdentifierFactory();
                var identifier = factory.Load("Core14.profile.xml"); // can be an absolute or relative path. Beware of 260 chars limitation of the path length in Windows. Linux allows 4096 chars.
                var languages = identifier.Identify(final);
                return new LanguageDetectionDataset(languages.FirstOrDefault().Item1.Iso639_3);
            }

        }

        return new LanguageDetectionDataset("UNKNOWN");
    }

    public Action<string, DatasetItem> Setup()
    {
        return (string html, DatasetItem item) => item.LanguageDetection = Process(html);
    }

}

public static class CrawlerExtensionLanguageDetection
{

    public static Crawler<T> Language<T>(this Crawler<T> crawler) where T : DatasetItem, new()
    {
        return crawler.Rebuild(new LanguageDetectionModule());
    }
}