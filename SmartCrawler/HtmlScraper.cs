namespace SmartCrawler;

public class HtmlScraper
{
    private static readonly HttpClient Client = new HttpClient();

    static HtmlScraper()
    {
        Client.Timeout = TimeSpan.FromSeconds(20);
    }

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