namespace SmartCrawler;

/**
* Compact version of the URL object
* It contains necessary metadata that informs the crawler whether to process this URL or not
*/
public struct CrawledSite : IAsyncQueueItem
{
    public readonly string Url;

    /// <summary>
    /// HtmlScraper can return an error when processing a specific file. However, that error might not be critical.
    /// This property determines how many retries are left
    /// </summary>
    public int Retries = 0;

    /// <summary>
    /// Determines how deep should a crawler go when it comes to visiting hyper links on the website
    /// </summary>
    public readonly int DepthsLeft;

    public CrawledSite(string url, int depthsLeft, int retries = 0)
    {
        Url = url;
        Retries = retries;
        DepthsLeft = depthsLeft;
    }

    public string GetKey()
    {
        return Url;
    }
}