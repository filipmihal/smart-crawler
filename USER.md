# User documentation

## About

SmartCrawler is a new kind of website crawler. Its main focus is on speed, scalability, and ease of use. You can just download the project, create an instance of the SmartCrawler, use any of the modules available and start scraping. Once you're done, just export the data to your favorite format.

## How to use SmartCrawler

1. Download the project
2. Set up Crawler options according to your needs
3. Create an instance of the SmartCrawler class
4. Append a list of URLs that you want to crawl or pick one from the sample list
5. Add modules that you want to use or create your own
6. Start the crawler
7. Export the data to a specific format. If your desired format is missing you can easily add it to the crawler.

> Sample code can be found in [Program.cs](./SmartCrawlerExample/Program.cs)

### 2. Crawler options

create a `CrawlerOptions` object and change the default setting as you wish.

Options

- CrawlingDepthOptions
  - `Crawling depth` (int) defines how deep in the tree structure of links should the crawler go. If it's set to `0` then only the URLs in the initial list are scraped.
  - `VisitCrossDomain` (bool) determines whether the crawler should visit cross-domain links as well.
- `ParallelCrawlers` (int) number of parallel crawlers
- `MaxRetries` (int) the maximum number of retries to request the content from an online resource

Example:

```C#
CrawlingDepthOptions depth = new CrawlingDepthOptions(crawlingDepth: 0, visitCrossDomain: false);
CrawlerOptions option = new CrawlerOptions(parallelCrawlers: 5, maxRetries: 3, depth);
```

### 3. 4. Create an instance of the crawler and append urls

Sometimes it might be quite difficult to find urls that contain data you want to scrape. I found Google API very helpful. I've created a simple [Jupyter notebook](./SmartCrawler/Urls/UrlSamples.cs) that uses Google API to get a list of URLs based on the entered query. There is also a `SampleUrls` object that contains sample URLs you can use right away!

```C#
SmartCrawler.Crawler crawler = new SmartCrawler.Crawler(option, UrlSamples.LawFirmsInSanFrancisco).Contacts(social: true);
```

or add your own list:

```C#
SmartCrawler.Crawler crawler = new SmartCrawler.Crawler(option,  new[]
    {
        "https://dolanlawfirm.com/",
        "https://www.cartwrightlaw.com/"
    });
```

### 5. Use built-in modules

You can add modules to your crawler in two ways. The first one is to add a list of modules when constructing the object, the other one, more elegant is to call the extension method on the object. Latter creates a new crawler that uses the same options, modules, and additionally the new module.

Examples:

Extension methods

```C#
SmartCrawler.Crawler crawler = new SmartCrawler.Crawler(option, UrlSamples.LawFirmsInSanFrancisco)
SmartCrawler.Crawler smartCrawler = crawler.Contacts(social: true).CryptoWallets();
```

Initial list

```C#
SmartCrawler.Crawler crawler = new SmartCrawler.Crawler(options, urls, new[]{contactsModule})
```

Right now, there are two modules available: Contacts and Crypto Wallets. However, the SmartCrawler was built in a way to make it super easy to add your own modules.

### 6. Start the crawler

You can start the crawler by running:

```C#
SmartCrawler.Crawler crawler = new SmartCrawler.Crawler(option, UrlSamples.LawFirmsInSanFrancisco).Contacts(social: true);
await crawler.StartAsync();
```

### 7. Export the data

You can either export the data using the SmartCrawler `Export` method or get the dataset using `GetFinalList` and use LINQ to post-process the scraped data.

Examples:

```C#
var crawledData = crawler.GetFinalList();


var emails = from url in crawledData
             where url.Contacts.Emails.Length > 0 && url.Contacts.LinkedIns.Length > 0 && url.Contacts.Facebooks.Length == 0
             select url.Contacts.Emails;
```

```C#
ExportOptions options = new ExportOptions();
crawler.ExportDataset(options, ExportType.Sql);
crawler.ExportDataset(options, ExportType.Json);
```

Export options:

- Filename
- Separator: How should the exported files be separated

## Add your own modules

In order to add your own modules, you need to create a dataset class and a module itself.

1.  Create a dataset class. It should represent the format of the data your module is going to return. Example can be found in [ContactsDataset.cs](./SmartCrawler/Modules/Contacts/ContactsDataset.cs)
2.  Add your dataset object to the general [DatasetItem](./SmartCrawler/Modules/DatasetItem.cs). Please, do not forget to mark the property as nullable as each module is optional.
3.  Create a module class. It must be inherited from `IBaseModuleSetup`
    - Process method receives the raw html, scrapes important data and returns the data in a form of a dataset object created in the first step
    - Setup method matches the processing method to the property of the general dataset
      Example can be found in [ContactsModule.cs](./SmartCrawler/Modules/Contacts/ContactsModule.cs)
4.  (Optional) Add an extension method to the SmartCrawler

    ```C#
    public static class CrawlerExtensionContacts
    {

        public static Crawler Contacts(this Crawler crawler, bool social)
        {
            return crawler.Rebuild(new ContactsModule(social));
        }

    }
    ```
