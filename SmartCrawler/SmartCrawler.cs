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