namespace SmartCrawler;

public struct CrawledSite
{
    public readonly string Url;
    public readonly int Retries = 0;
    public readonly int DepthsLeft;

    public CrawledSite(string url, int retries, int depthsLeft)
    {
        Url = url;
        Retries = retries;
        DepthsLeft = depthsLeft;
    }
}