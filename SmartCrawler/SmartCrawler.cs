using System.Text.RegularExpressions;

namespace SmartCrawler;

public class SmartCrawler
{
    private readonly int _numberOfConcurrentCrawlers;
    private readonly int _linkDepth;
    private readonly CrawledSite[] _initialUrlList;
    private readonly AsyncStorage<CrawledSite> _storage = new AsyncStorage<CrawledSite>();


    public SmartCrawler(int numberOfThreads, int linkDepth, string[] initialUrlArray)
    {
        _numberOfConcurrentCrawlers = numberOfThreads;
        _linkDepth = linkDepth;
        _initialUrlList = ConvertUrlArrayToList(initialUrlArray);
    }

    private static CrawledSite[] ConvertUrlArrayToList(string[] urls)
    {
        CrawledSite[] list = new CrawledSite[urls.Length];
        for (int urlIndex = 0; urlIndex < urls.Length; urlIndex++)
        {
            // todo depths left
            list[urlIndex] = new CrawledSite(urls[urlIndex], 0);
        }

        return list;
    }
    private async Task Crawl(AsyncQueue<CrawledSite> queue, AsyncStorage<CrawledSite> storage, ThreadState threadState, int threadId)
    {
        while (true)
        {
            CrawledSite siteToCrawl;
            try
            {
                siteToCrawl = queue.Dequeue();
                threadState.UpdateState(threadId, false);
            }
            catch (InvalidOperationException)
            {
                if (threadState.UpdateAndCanFinish(threadId, true))
                {
                    return;
                }
                continue;
            }
            ScraperResponse response = await HtmlScraper.ScrapeUrl(siteToCrawl.Url);
            // TODO: check retries
            // TODO: add links to the queue
            if (response.IsSuccessful)
            {
                storage.Add(siteToCrawl);
            }
        }
    }

    public static List<CrawledSite> FilterAndFormatUrls(List<string> rawUrls, int newDepth, bool crossDomain, string parentUrl)
    {
        List<CrawledSite> sitesToCrawl = new List<CrawledSite>();
        Uri parentUri = new Uri(parentUrl);
        foreach (var url in rawUrls)
        {
            if (!crossDomain)
            {
                Uri childUri = new Uri(url);
                if (childUri.Host != parentUri.Host)
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



    public async Task StartAsync()
    {
        AsyncQueue<CrawledSite> queue = new AsyncQueue<CrawledSite>(_initialUrlList);
        ThreadState threadState = new ThreadState(_numberOfConcurrentCrawlers);

        List<Task> tasks = new List<Task>();

        for (var i = 0; i < _numberOfConcurrentCrawlers; i++)
        {
            int threadId = i;
            tasks.Add(Task.Run(async () => await Crawl(queue, _storage, threadState, threadId)));
        }

        await Task.WhenAll(tasks);
    }

    public List<CrawledSite> GetFinalList()
    {
        return _storage.Export();
    }
}