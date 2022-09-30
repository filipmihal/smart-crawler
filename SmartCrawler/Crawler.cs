using SmartCrawler.Exports;
using SmartCrawler.Modules;

namespace SmartCrawler;

/**
 * Main logic of the SmartCrawler
 * Consult programming docs for further details of how this class works
 */
public class Crawler<T> where T : DatasetItem, new()
{
    private readonly CrawlerOptions _options;

    /// <summary>
    /// Crawler uses CrawledSites as a unit of inforamtion
    /// </summary>
    private readonly CrawledSite[] _initialUrlList;
    private readonly AsyncStorage<T> _storage = new AsyncStorage<T>();
    /// <summary>
    /// _modules are recommended to have immutable nature.
    /// There are many cases when a user wants to have multiple crawlers sharing the same options but containing different modules
    /// </summary>
    private readonly IBaseModule[]? _modules;
    private readonly CrawlerStatus _status = new CrawlerStatus();

    public Crawler(CrawlerOptions options, string[] initialUrlArray, IBaseModule[]? modules = null)
    {
        _modules = modules;
        _options = options;
        _initialUrlList = ConvertInitialUrlArrayToList(initialUrlArray);
    }

    /// <summary>
    /// Initialize the crawler with a list of modules
    /// Extension methods can be still used
    /// </summary>
    private Crawler(CrawlerOptions options, CrawledSite[] sites, IBaseModule[]? modules = null)
    {
        _initialUrlList = sites;
        _options = options;
        _modules = modules;
    }

    /// <summary>
    /// Creates a new Crawler instance. Keeps the original options object linked to the new instance
    /// and creates a new module list that contain the <see cref="newModule"/>
    /// </summary>
    /// <param name="newModule">Module that will be added to the new Crawler instance</param>
    /// <remarks>
    /// This method should be only used in the module extension methods.
    /// Be aware of its immutable and mutable properties.
    /// The inheritance of the options object is done on purpose, so all crawlers that were rebuilt from the original one can be changed easily.
    /// </remarks>
    public Crawler<T> Rebuild(IBaseModule newModule)
    {
        var newModules = _modules != null ? _modules.Concat(new[] { newModule }).ToArray() : new[] { newModule };

        return new Crawler<T>(_options, _initialUrlList, newModules);
    }

    /// <summary>
    /// Converts the url string array to <see cref="CrawledSite"/> array
    /// </summary>
    /// <param name="urls">URLs to be converted</param>
    /// <remarks>
    /// Prefills all new objects with the right values from the <see cref="_options"/> property
    /// </remarks>
    private CrawledSite[] ConvertInitialUrlArrayToList(string[] urls)
    {
        CrawledSite[] list = new CrawledSite[urls.Length];
        for (int urlIndex = 0; urlIndex < urls.Length; urlIndex++)
        {
            // if crawling depth is not set, it means that the crawler shall not visit any additional links
            int crawlingDepth = _options.DepthOptions?.CrawlingDepth ?? 0;
            // These variables must be set for each CrawledSite separately because their values are bound to the URL and might change during the crawling process
            list[urlIndex] = new CrawledSite(urls[urlIndex], crawlingDepth, _options.MaxRetries);
        }

        return list;
    }

    /// <summary>
    /// Single crawling unit
    /// Contains all the necessary logic to crawl given sites
    /// </summary>
    /// <param name="uniqueQueue">Unique queue that stores all CrawledSite objects that still needs to be crawled</param>
    /// <param name="storage">Mutable storage. Each crawler should have access to it</param>
    /// <param name="threadState">Provides access to the states of other threads</param>
    /// <param name="threadId">Identifier of the crawler</param>
    /// <param name="actions">Module actions that should be executed for each URL</param>
    /// <remarks>
    /// Manages thread state
    /// Works with the global unique queue
    /// Calls <see cref="HtmlScraper"/> and updates the current CrawledSite object in case of any errors 
    /// Contains the main logic for adding new urls to the unique queue. It parses all links on a given website.
    /// Processes modules and adds the data to the global storage
    /// </remarks>
    private async Task Crawl(AsyncUniqueQueue<CrawledSite> uniqueQueue, AsyncStorage<T> storage, ThreadState threadState, int threadId, Action<string, T>[] actions)
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

            T datasetItem = new T();
            datasetItem.Url = siteToCrawl.Url;

            if (siteToCrawl.DepthsLeft > 0 && _options.DepthOptions.HasValue)
            {
                List<string> links = CrawlerHelpers.ParseLinks(response.Html, siteToCrawl.Url);
                List<CrawledSite> sites = CrawlerHelpers.FilterAndFormatUrls(links, siteToCrawl.DepthsLeft - 1,
                    _options.DepthOptions.Value.VisitCrossDomain, siteToCrawl.Url);
                uniqueQueue.EnqueueList(sites);
                datasetItem.SubUrls = links.ToArray();
                datasetItem.Depth = _options.DepthOptions.Value.CrawlingDepth - siteToCrawl.DepthsLeft;
            }

            ProcessModules(actions, response.Html, datasetItem);

            storage.Add(datasetItem);
        }

    }

    private void ProcessModules(Action<string, T>[] actions, string html, T item)
    {

        foreach (var action in actions)
        {
            action(html, item);
        }
    }

    /// <summary>
    /// Creates an array of actions to be executed
    /// </summary>
    private Action<string, T>[] SetupModules()
    {
        if (_modules != null)
        {
            /**
             * This might seem like a redundant step but Crawl method should contain as little logic as possible and the idea of actions
             * was introduced because modules are not the only logic that is executed on the Html content.
            */
            Action<string, T>[] actions = new Action<string, T>[_modules.Length];
            for (int i = 0; i < _modules.Length; i++)
            {
                actions[i] = _modules[i].Setup();
            }

            return actions;

        }

        return Array.Empty<Action<string, T>>();

    }

    /// <summary>
    /// Starts the crawling process. Executes the given number of parallel tasks
    /// </summary>
    /// <remarks>
    /// Initializes <see cref="UniqueQueue"/> and <see cref="ThreadState"/>
    /// Cannot call getFinalList() while the main crawling process is running
    /// </remarks>

    public async Task StartAsync()
    {
        lock (_status)
        {
            _status.CrawlingInProgress = true;
        }
        AsyncUniqueQueue<CrawledSite> uniqueQueue = new AsyncUniqueQueue<CrawledSite>(_initialUrlList);
        ThreadState threadState = new ThreadState(_options.ParallelCrawlers);
        Action<string, T>[] actions = SetupModules();

        List<Task> tasks = new List<Task>();

        for (var i = 0; i < _options.ParallelCrawlers; i++)
        {
            int threadId = i;
            tasks.Add(Task.Run(() => Crawl(uniqueQueue, _storage, threadState, threadId, actions)));
        }

        await Task.WhenAll(tasks);
        lock (_status)
        {
            _status.CrawlingInProgress = false;
        }
    }

    public List<T> GetFinalList()
    {
        lock (_status)
        {
            if (_status.CrawlingInProgress)
            {
                throw new CrawlingInProgressException();
            }
        }
        return _storage.Export();
    }

    public void ExportDataset(ExportOptions exportOptions, ExportType type)
    {
        var exportService = ExportFactory<T>.GenerateExport(type, exportOptions);
        exportService.Export(GetFinalList());
    }
}

public class CrawlingInProgressException : Exception { }