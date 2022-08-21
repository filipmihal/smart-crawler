namespace SmartCrawler;

public class HtmlScraper
{
    private readonly HttpClient _client = new HttpClient();

    public async Task<ScraperResponse> ScrapeUrl(string url)
    {
        return new ScraperResponse(true, await _client.GetStringAsync(url));
    }
    
}