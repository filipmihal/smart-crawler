using System.Text.RegularExpressions;

namespace SmartCrawler;

public class SmartCrawler
{
    private readonly CrawlerOptions _options;
    private readonly CrawledSite[] _initialUrlList;
    private readonly AsyncStorage<CrawledSite> _storage = new AsyncStorage<CrawledSite>();


    public SmartCrawler(CrawlerOptions options, string[] initialUrlArray)
    {
        _options = options;
        _initialUrlList = ConvertInitialUrlArrayToList(initialUrlArray);
    }

    private CrawledSite[] ConvertInitialUrlArrayToList(string[] urls)
    {
        CrawledSite[] list = new CrawledSite[urls.Length];
        for (int urlIndex = 0; urlIndex < urls.Length; urlIndex++)
        {
            int crawlingDepth = _options.DepthOptions?.CrawlingDepth ?? 0;
            list[urlIndex] = new CrawledSite(urls[urlIndex], crawlingDepth, _options.MaxRetries);
        }

        return list;
    }
    private async Task Crawl(AsyncUniqueQueue<CrawledSite> uniqueQueue, AsyncStorage<CrawledSite> storage, ThreadState threadState, int threadId)
    {
        while (true)
        {
            CrawledSite siteToCrawl;
            try
            {
                siteToCrawl = uniqueQueue.Dequeue();
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
            if (!response.IsSuccessful)
            {
                if (!response.IsCritical)
                {
                    if (siteToCrawl.Retries > 0)
                    {
                        siteToCrawl.Retries -= 1;
                        uniqueQueue.Enqueue(siteToCrawl);
                    }
                }

                continue;

            }
            if (siteToCrawl.DepthsLeft > 0 && _options.DepthOptions.HasValue)
            {
                List<string> links = ParseLinks(response.Html, siteToCrawl.Url);
                List<CrawledSite> sites = FilterAndFormatUrls(links, siteToCrawl.DepthsLeft - 1, _options.DepthOptions.Value.VisitCrossDomain, siteToCrawl.Url);
                uniqueQueue.EnqueueList(sites);

            }
            storage.Add(siteToCrawl);
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



    public async Task StartAsync()
    {
        AsyncUniqueQueue<CrawledSite> uniqueQueue = new AsyncUniqueQueue<CrawledSite>(_initialUrlList);
        ThreadState threadState = new ThreadState(_options.ParallelCrawlers);

        List<Task> tasks = new List<Task>();

        for (var i = 0; i < _options.ParallelCrawlers; i++)
        {
            int threadId = i;
            tasks.Add(Task.Run(async () => await Crawl(uniqueQueue, _storage, threadState, threadId)));
        }

        await Task.WhenAll(tasks);
    }

    public List<CrawledSite> GetFinalList()
    {
        return _storage.Export();
    }
}