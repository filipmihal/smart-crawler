using SmartCrawler;
using SmartCrawler.Exports;
using SmartCrawler.Modules.Contacts;
using SmartCrawler.Urls;


// 1. Set Crawling options 
CrawlingDepthOptions depth = new CrawlingDepthOptions(1, false);
CrawlerOptions option = new CrawlerOptions(5, 3, depth);

//2. Setup the SmartCrawler
SmartCrawler.Crawler crawler = new SmartCrawler.Crawler(option, UrlSamples.LawFirmsInSanFrancisco).Contacts(social: true);
await crawler.StartAsync();

//3. Get the list of crawled data and analyse the results via Linq
var crawledData = crawler.GetFinalList();


// todo select title as well
var emails = from url in crawledData
    where url.Contacts.Value.Emails.Length > 0 && url.Contacts.Value.LinkedIns.Length > 0 && url.Contacts.Value.Facebooks.Length == 0
    select url.Contacts.Value.Emails;

string message = $"Dear sir or madam," +
                 $"I am writing you regarding a court case that I am involved in. I've been contacted by IRS regarding some discrepancies in my company's investment fund." +
                 $"I would like to use your services and discuss my future steps." +
                 $"Sincerely," +
                 $"John Smith.";

// todo: send emails


// todo: export to json
// Console.WriteLine(a[0].Contacts.Value.Emails);
ExportOptions options = new ExportOptions();
crawler.ExportDataset(options, ExportType.Json);
crawler.ExportDataset(options, ExportType.Sql);

// SmartCrawler.SmartCrawler.ParseLinks("<a href=\"ahoj\"></a> ddddd <a hraf='cau'>ddd</a>","https://docs.microsoft.com/sk-sk");
// Json.Export(crawler.GetFinalList());

