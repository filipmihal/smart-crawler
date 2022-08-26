namespace SmartCrawler;

public struct CrawledSite
{
    public readonly string Url;
    public readonly int Retries = 0;
    public readonly int DepthsLeft;

    public CrawledSite(string url, int depthsLeft, int retries = 0)
    {
        Url = url;
        Retries = retries;
        DepthsLeft = depthsLeft;
    }
}