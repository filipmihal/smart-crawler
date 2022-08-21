namespace SmartCrawler;

public class HtmlScraper
{
    private readonly HttpClient _client = new HttpClient();

    public async Task<ScraperResponse> ScrapeUrl(string url)
    {
        try
        {
            string html = await _client.GetStringAsync(url);
            return new ScraperResponse(true, false, html);
        }
        catch (TaskCanceledException e)
        {
            Console.WriteLine($"[HtmlScraper] Warning: {url} was timed out!");
            return new ScraperResponse(false, false, "");
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine($"[HtmlScraper] Warning: {url} is not a valid URL!");
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"[HtmlScraper] Warning: Connectivity issue! {url} skipped");
        }
        return new ScraperResponse(false, true, "");
    }
}