namespace SmartCrawler;
using System.Text.RegularExpressions;


public static class CrawlerHelpers
{
    /// <summary>
    /// Filters cross-domain urls if <see cref="crossDomain"/> is set to true and formats the rest to a form of a <see cref="CrawledSite"/> object
    /// </summary>
    /// <param name="rawUrls">
    /// Array of urls that needs to be filtered and formatted. Be aware that the urls in the array must be of the same crawling level. 
    /// The will be given the same depth.
    /// </param>
    /// <param name="newDepth">New depth is the same for all urls</param>
    /// <param name="crossDomain">Boolean that determines whether to </param>
    /// <param name="parentUrl">Url according to which the filtering is performed</param>
    /// <remarks>
    /// subdomains are considered as a subset of the domain. Hence, they are not filtered.
    /// </remarks>
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
            //TODO: what about retries??
            sitesToCrawl.Add(new CrawledSite(url, newDepth));
        }

        return sitesToCrawl;
    }

    /// <summary>
    /// Parses links from a given string (Html)
    /// Only valid links are parsed. Links that navigate to the current url are avoided
    /// </summary>
    /// <param name="inputString">raw html content</param>
    /// <param name="parentUrl">Url from which the html content originates</param>
    /// <remarks>
    /// This method also processes relative links and appends the correct uri to the paths
    /// </remarks>
    public static List<string> ParseLinks(string inputString, string parentUrl)
    {
        List<string> links = new List<string>();
        Uri uri = new Uri(parentUrl);
        foreach (Match match in Regex.Matches(inputString, "<a\\s+(?:[^>]*?\\s+)?href=([\"\'])(.*?)[\"\']"))
        {
            string link = match.Groups[2].ToString();

            if (link.StartsWith("http") || link.StartsWith("www.") )
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