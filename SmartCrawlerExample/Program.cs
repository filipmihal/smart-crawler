using SmartCrawler;
using SmartCrawler.Exports;
using SmartCrawler.Modules;
using SmartCrawler.Modules.Contacts;
using SmartCrawler.Modules.CryptoWallets;
using SmartCrawler.Modules.LanguageDetection;

using SmartCrawler.Urls;
using SmartCrawlerExample;


// 1. Set Crawling options 
CrawlingDepthOptions depth = new CrawlingDepthOptions(crawlingDepth: 0, visitCrossDomain: false);
CrawlerOptions option = new CrawlerOptions(parallelCrawlers: 5, maxRetries: 3, depth);

//2. Setup the SmartCrawler
SmartCrawler.Crawler<DatasetItem> crawler = new SmartCrawler.Crawler<DatasetItem>(option, UrlSamples.LawFirmsInSanFrancisco).Contacts(social: true);
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
ExportOptions options = new ExportOptions() { Filename = "emails" };
crawler.ExportDataset(options, ExportType.Sql);
crawler.ExportDataset(options, ExportType.Json);
ExportOptions optionsCSV = new ExportOptions() { Filename = "emails", Separator = UrlExportSeparator.Url };

crawler.ExportDataset(optionsCSV, ExportType.Csv);


// CSV EXPORT EXAMPLE
CrawlingDepthOptions depth2 = new CrawlingDepthOptions(crawlingDepth: 2, visitCrossDomain: true);
CrawlerOptions option2 = new CrawlerOptions(parallelCrawlers: 7, maxRetries: 1, depth);

//2. Setup the SmartCrawler
SmartCrawler.Crawler<DatasetItem> crawler2 = new SmartCrawler.Crawler<DatasetItem>(option, UrlSamples.CateringCompaniesInLondon, new[] { new CryptoWalletsModule() });
await crawler2.StartAsync();

ExportOptions options2 = new ExportOptions() { Filename = "crypto", Separator = UrlExportSeparator.Url };
crawler2.ExportDataset(options2, ExportType.Csv);

// You must download the core.xml to run this
// SmartCrawler.Crawler<DatasetItem> crawlerLang = new SmartCrawler.Crawler<DatasetItem>(option, new[] { "https://mfts.io/" }).Language();
// await crawlerLang.StartAsync();
// var languages = crawlerLang.GetFinalList();
// Console.WriteLine(languages[0].LanguageDetection.Language);


