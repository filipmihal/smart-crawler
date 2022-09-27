namespace SmartCrawler;

/**
* Wrapper around HttpClient
* Logs critical and non-critical errors 
*/
public class HtmlScraper
{
    /// <summary>
    /// Built-in http client that makes http requests to the online resources
    /// </summary>
    /// <remarks>
    /// We create a single instance for the whole program, so we avoid port exhaustion,
    /// which might occur when creating too many HttpClient instances
    /// </remarks>
    private static readonly HttpClient Client = new HttpClient();

    static HtmlScraper()
    {
        // This is a quick win to decrease loading times. Most of the content loads much faster. 
        // In case a user wants to change this property I might add it to the CrawlerOptions class. 
        Client.Timeout = TimeSpan.FromSeconds(20);
    }

    /// <summary>
    /// Wraps GetStringAsync and logs all errors instead of throwing them
    /// </summary>
    /// <param name="url">url of the resource that should be scraped</param>
    /// <remarks>
    /// TaskCanceledException AKA timeout is considered as a non-critical error
    /// Returns a <see cref="ScraperResponse"/> object which contains the html content and metadata about the status of the request 
    /// </remarks>
    public static async Task<ScraperResponse> ScrapeUrl(string url)
    {
        try
        {
            string html = await Client.GetStringAsync(url);
            return new ScraperResponse(true, false, html);
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine($"[HtmlScraper] Warning: {url} was timed out!");
            return new ScraperResponse(false, false, "");
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"[HtmlScraper] Warning: {url} is not a valid URL!");
        }
        catch (HttpRequestException)
        {
            Console.WriteLine($"[HtmlScraper] Warning: Connectivity issue! {url} skipped");
        }
        return new ScraperResponse(false, true, "");
    }
}