using SmartCrawler;
using SmartCrawler.Exports;
using SmartCrawler.Modules.Contacts;
using SmartCrawler.Urls;
using SmartCrawlerExample;


// 1. Set Crawling options 
CrawlingDepthOptions depth = new CrawlingDepthOptions(crawlingDepth: 0, visitCrossDomain: false);
CrawlerOptions option = new CrawlerOptions(parallelCrawlers: 5, maxRetries: 3, depth);

//2. Setup the SmartCrawler
SmartCrawler.Crawler crawler = new SmartCrawler.Crawler(option, UrlSamples.LawFirmsInSanFrancisco).Contacts(social: true);
await crawler.StartAsync();

//3. Get the list of crawled data and analyse the results via Linq
var crawledData = crawler.GetFinalList();


var emails = from url in crawledData
             where url.Contacts.Emails.Length > 0 && url.Contacts.LinkedIns.Length > 0 && url.Contacts.Facebooks.Length == 0
             select url.Contacts.Emails;

string message = $"Dear sir or madam," +
                 $"I am writing you regarding a court case that I am involved in. I've been contacted by IRS regarding some discrepancies in my company's investment fund." +
                 $"I would like to use your services and discuss my future steps." +
                 $"Sincerely," +
                 $"John Smith.";

foreach (var emailsOnUrl in emails)
{
    Mock.SendEmail(emailsOnUrl[0], message);
}

// export to JSON and SQL
ExportOptions options = new ExportOptions(){Filename = "emails"};
crawler.ExportDataset(options, ExportType.Sql);
crawler.ExportDataset(options, ExportType.Json);
var a = crawler.GetFinalList();
Console.WriteLine("Ahoj");

