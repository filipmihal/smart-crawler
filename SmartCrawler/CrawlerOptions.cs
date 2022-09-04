namespace SmartCrawler;

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
    public int ParallelCrawlers;
    public int MaxRetries;

    public CrawlerOptions(int parallelCrawlers = 1, int maxRetries = 0, CrawlingDepthOptions? depthOptions = null)
    {
        DepthOptions = depthOptions;
        ParallelCrawlers = parallelCrawlers;
        MaxRetries = maxRetries;
    }
}