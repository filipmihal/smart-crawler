using SmartCrawler.Exports;
using SmartCrawler.Modules;

namespace SmartCrawler;

/**
 * Main logic of the SmartCrawler
 * Consult docs for further details of how this class works
 */
public class Crawler
{
    private readonly CrawlerOptions _options;
    private readonly CrawledSite[] _initialUrlList;
    private readonly AsyncStorage<DatasetItem> _storage = new AsyncStorage<DatasetItem>();
    private readonly IBaseModule[]? _modules;
    private readonly CrawlerStatus _status = new CrawlerStatus();

    public Crawler(CrawlerOptions options, string[] initialUrlArray, IBaseModule[]? modules = null)
    {
        _modules = modules;
        _options = options;
        _initialUrlList = ConvertInitialUrlArrayToList(initialUrlArray);
    }

    private Crawler(CrawlerOptions options, CrawledSite[] sites,  IBaseModule[]? modules = null)
    {
        _initialUrlList = sites;
        _options = options;
        _modules = modules;
    }
    
    /// <summary>
    /// Creates a new Crawler instance. Keeps the original options object linked to the new instance and creates a new module list that contain the <see cref="newModule"/>
    /// </summary>
    /// <param name="newModule">Module that will be added to the new Crawler instance</param>
    /// <remarks>
    /// This method should be only used in the module extension methods.
    /// Be aware of its immutable and mutable properties.
    /// The inheritance of the options object is done on purpose, so all crawlers that were rebuilt can be changed easily.
    /// </remarks>
    public Crawler Rebuild(IBaseModule newModule)
    {
        var newModules = _modules != null ? _modules.Concat(new[] { newModule }).ToArray() : new[] { newModule };

        return new Crawler(_options, _initialUrlList, newModules);
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
    private async Task Crawl(AsyncUniqueQueue<CrawledSite> uniqueQueue, AsyncStorage<DatasetItem> storage, ThreadState threadState, int threadId, Action<string, DatasetItem>[] actions)
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

                DatasetItem datasetItem = new DatasetItem(siteToCrawl.Url);

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

    private void ProcessModules(Action<string, DatasetItem>[] actions, string html, DatasetItem item)
    {
        
            foreach (var action in actions)
            {
                action(html, item);
            }
    }

    private Action<string, DatasetItem>[] SetupModules()
    {
        if (_modules != null)
        {
            Action<string, DatasetItem>[] actions = new Action<string, DatasetItem>[_modules.Length];
            for (int i = 0; i < _modules.Length; i++)
            {
                actions[i] = _modules[i].Setup();
            }

            return actions;

        }

        return Array.Empty<Action<string, DatasetItem>>();

    }
    public async Task StartAsync()
    {
        lock (_status)
        {
            _status.CrawlingInProgress = true;
        }
        AsyncUniqueQueue<CrawledSite> uniqueQueue = new AsyncUniqueQueue<CrawledSite>(_initialUrlList);
        ThreadState threadState = new ThreadState(_options.ParallelCrawlers);
        Action<string, DatasetItem>[] actions = SetupModules();

        List<Task> tasks = new List<Task>();

        for (var i = 0; i < _options.ParallelCrawlers; i++)
        {
            int threadId = i;
            tasks.Add(Task.Run(async () => await Crawl(uniqueQueue, _storage, threadState, threadId, actions)));
        }

        await Task.WhenAll(tasks);
        lock (_status)
        {
            _status.CrawlingInProgress = false;
        }
    }

    public List<DatasetItem> GetFinalList()
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
        var exportService = ExportFactory<DatasetItem>.GenerateExport(type, exportOptions);
        exportService.Export(GetFinalList());
    }
}

public class CrawlingInProgressException: Exception{}