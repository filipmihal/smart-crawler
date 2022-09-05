namespace SmartCrawler;
using System.Text.RegularExpressions;


public static class CrawlerHelpers
{
    public static List<CrawledSite> FilterAndFormatUrls(List<string> rawUrls, int newDepth, bool crossDomain, string parentUrl)
    {
        List<CrawledSite> sitesToCrawl = new List<CrawledSite>();
        Uri parentUri = new Uri(parentUrl);
        foreach (var url in rawUrls)
        {
            if (!crossDomain)
            {
                Uri childUri = new Uri(url);

                if (childUri.CleanHost() != parentUri.CleanHost())
                {
                    continue;
                }
            }
            sitesToCrawl.Add(new CrawledSite(url, newDepth));
        }

        return sitesToCrawl;
    }

    public static List<string> ParseLinks(string inputString, string parentUrl)
    {
        List<string> links = new List<string>();
        Uri uri = new Uri(parentUrl);
        foreach (Match match in Regex.Matches(inputString, "<a\\s+(?:[^>]*?\\s+)?href=([\"\'])(.*?)[\"\']"))
        {
            string link = match.Groups[2].ToString();

            if (link.StartsWith("http"))
            {
                links.Add(link);
            }
            else if (link.StartsWith("/"))
            {
                links.Add($"{uri.Scheme}://{uri.Host}{link}");
            }
            else
            {
                if (uri.AbsolutePath.EndsWith("/"))
                {
                    links.Add($"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}{link}");
                }
                else
                {
                    links.Add($"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}/{link}");
                }
            }
        }

        return links;
    }
}