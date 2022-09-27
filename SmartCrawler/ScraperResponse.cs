namespace SmartCrawler;

/**
* Html content with metadata about the status of the http request
*/
public struct ScraperResponse
{
    public readonly bool IsSuccessful;

    /// <summary>
    /// Determines whether the error that resulted from the request was critical or not 
    /// Based on that information the value of MaxRetries is updated
    /// </summary>
    public readonly bool IsCritical;
    public readonly string Html;

    public ScraperResponse(bool isSuccessful, bool isCritical, string html)
    {
        this.IsSuccessful = isSuccessful;
        this.IsCritical = isCritical;
        this.Html = html;
    }
}