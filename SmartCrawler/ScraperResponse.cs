namespace SmartCrawler;

public struct ScraperResponse
{
    public readonly bool IsSuccessful;
    public readonly string Html;

    public ScraperResponse(bool isSuccessful, string html)
    {
        this.IsSuccessful = isSuccessful;
        this.Html = html;
    }
}