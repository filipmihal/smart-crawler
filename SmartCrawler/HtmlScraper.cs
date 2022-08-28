namespace SmartCrawler;

public class HtmlScraper
{
    public static async Task<ScraperResponse> ScrapeUrl(string url)
    {
        try
        {
            HttpClient client = new HttpClient();
            string html = await client.GetStringAsync(url);
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