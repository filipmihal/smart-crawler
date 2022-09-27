namespace SmartCrawler;

/**
* Depth options form a single setup object
*/
public struct CrawlingDepthOptions
{
    public int CrawlingDepth;
    public bool VisitCrossDomain;

    public CrawlingDepthOptions(int crawlingDepth, bool visitCrossDomain)
    {
        CrawlingDepth = crawlingDepth;
        VisitCrossDomain = visitCrossDomain;
    }
}

public struct CrawlerOptions
{
    public CrawlingDepthOptions? DepthOptions;
    
    /// <summary>
    /// Number of tasks that will be ran in parallel
    /// </summary>
    public int ParallelCrawlers;

    /// <summary>
    /// the maximum number of requests to the same URL that can result with a noncritical error
    /// </summary>
    public int MaxRetries;

    public CrawlerOptions(int parallelCrawlers = 1, int maxRetries = 0, CrawlingDepthOptions? depthOptions = null)
    {
        DepthOptions = depthOptions;
        ParallelCrawlers = parallelCrawlers;
        MaxRetries = maxRetries;
    }
}