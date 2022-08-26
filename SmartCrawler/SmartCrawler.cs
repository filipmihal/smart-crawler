namespace SmartCrawler;

public class SmartCrawler
{
    private readonly int _threads;
    private readonly int _linkDepth;
    private readonly CrawledSite[] _initialUrlList;
    private readonly AsyncStorage<CrawledSite> _storage = new AsyncStorage<CrawledSite>();


    public SmartCrawler(int threads, int linkDepth, string[] initialUrlArray)
    {
        _threads = threads;
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
    private async void Crawl(AsyncQueue<CrawledSite> queue, AsyncStorage<CrawledSite> storage, ThreadState threadState, int threadId)
    {
        while (true)
        {
            CrawledSite siteToCrawl;
            try
            {
                siteToCrawl = queue.Dequeue();
                threadState.UpdateState(threadId, false);
            }
            catch (InvalidOperationException e)
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

    public void Start(int numberOfThreads)
    {
        AsyncQueue<CrawledSite> queue = new AsyncQueue<CrawledSite>(_initialUrlList);
        ThreadState threadState = new ThreadState(numberOfThreads);
        for (int threadId = 0; threadId < numberOfThreads; threadId++)
        {
            var crawlingThread = new Thread(() => Crawl(queue, _storage, threadState, threadId));
            crawlingThread.Start();   
        }
    }

    public List<CrawledSite> GetFinalList()
    {
        return _storage.Export();
    }
}