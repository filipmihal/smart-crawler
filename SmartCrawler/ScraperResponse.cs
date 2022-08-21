namespace SmartCrawler;

public struct ScraperResponse
{
    public readonly bool IsSuccessful;
    public readonly bool IsCritical;
    public readonly string Html;

    public ScraperResponse(bool isSuccessful, bool isCritical, string html)
    {
        this.IsSuccessful = isSuccessful;
        this.IsCritical = isCritical;
        this.Html = html;
    }
}