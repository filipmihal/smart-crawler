# Contribute (Programming documentation)

When developing SmartCrawler, my main focus was on the speed of the crawler and on providing the best developer experience for those who will use it. A user (developer) should be able to spend a max of 15 minutes reading the documentation and have a working crawler ready in max 5 simple steps. Additionally, the scraper shall not only be used on simple crawling tasks but should be easily scalable and serve as a base for complex software projects.

## Scraping on the low level

To get the content from online resources, SmartCrawler uses `HttpClient` library. It is a built-in DOTNET library that makes HTTP requests to given URLs and provides a raw response body. The reason I chose this library is that it is fast, easy to use, and thread-safe. To improve the user experience I built a wrapper around HttpClient called [HtmlScraper](./SmartCrawler/HtmlScraper.cs). Throughout the whole crawling process, we use a single HttpClient (as is recommended in the official documentation) to not cause port exhaustion.

### Webpage rendering

Right now, SmartCrawler does not support the rendering of scraped websites. That might lead to unwanted data loss. Arguably, most of the websites that provide interesting data for scraping are static websites or use SSR, so using an http-based scraping is a convenient solution. In the future, we might implement a webpage rendering functionality to support a wider range of possibly scraped websites. However, it will rapidly increase the crawling time.

## Thread-safe data structures

The essential feature of this project is parallelism. Hence, it is quite intuitive that we have to implement thread-safe data structures in order to avoid any race data, race conditions, or deadlocks. SmartCrawler uses multiple async data structures. They are located in [Structures](./SmartCrawler/Structures/) folder.

- [AsyncUniqueQueue](./SmartCrawler/Structures/AsyncUniqueQueue.cs)
  - It is a thread-safe queue
  - Additionally, it uses an internal HashSet to store items that were already enqueued.
  - The main idea is to only enqueue unique items
  - SmartCrawler uses it to store all URLs that are needed to be crawled. The data structure is built in a way that every URL will be scraped once and the crawler will avoid any loops. This saves time and keeps the final dataset in a clean state.
- [AsyncStorage](./SmartCrawler/Structures/AsyncStorage.cs)
  - Simple thread-safe storage
  - SmartCrawler uses it to save scraped data
- [ThreadState](./SmartCrawler/Structures/ThreadState.cs)
  - Stores states of all threads that are being used and provides a simple API to access states of all threads simultaneously
  - It is an extremely important part of the project. Its purpose is going to be explained later in the text.

## Parallel Crawling

In order to run multiple crawlers in parallel, we use `System.Threading.Tasks`. The logic for parallel crawling is included in the main [Crawler.cs](./SmartCrawler/Crawler.cs) file. Basically, we create numerous independent tasks and then use `Task.WhenAll` to execute them. All shared data structures that are used by the crawling tasks must be thread-safe. The logic of a single crawler is located in the `Crawl` method. `StartAsync` creates and runs all the tasks.

### Thread state management

One issue that arises when using parallel crawlers is that crawlers' states are dependent on each other. A single crawler can stop crawling only if itself and every other crawler are in the finished state. Otherwise, there might come a new batch of URLs that should be processed.
Therefore, I implemented [ThreadState data structure](./SmartCrawler/Structures/ThreadState.cs) in the `Crawl` method.

## Modules

The most distinctive feature of SmartCrawler is its Modules system. It was arguably the most challenging engineering task of the project. The main idea is to have independent sub-scrapers that scrape specific types of data from a website and save that data in a single object called [DatasetItem](./SmartCrawler/Modules/DatasetItem.cs). Each module has an access to raw html code and must satisfy [BaseModule](./SmartCrawler/Modules/BaseModule.cs) interface. A user should be able to easily select which modules he wants to use in his program.

### Module structure

A module must contain a processing function, a setup function, and a module dataset class.

- Module dataset class
  - Structure of data that is scraped by the module
- Processing function
  - Contains the scraping logic and returns module dataset class
- Setup
  - Matches a DatasetItem property to the result of the processing function

### Adding modules to SmartCrawler

Adding modules to the crawler must be as smooth as possible. Crawler object contains an internal list of modules that are run for every URL. There are two ways to do it. The first one is to insert a list of modules during the crawler initialization. The other one is to create an extension method that will create a copy of the crawler with the module included in the internal module list.

Extension method:

```C#
public static class CrawlerExtensionContacts
{

    public static Crawler Contacts(this Crawler crawler, bool social)
    {
        return crawler.Rebuild(new ContactsModule(social));
    }
}
```

Examples:

- [Contacts module](./SmartCrawler/Modules/Contacts/)
  - Returns emails, phone numbers, and socials such as LinkedIn and Facebook links and
- [CryptoWallets module](./SmartCrawler/Modules/CryptoWallets/)

### Final dataset

The final dataset must have a strongly typed nature, so users can easily access its properties or use tools such as LINQ. I created `DatasetItem` class that contains all module structs as nullable properties. It might not be an ideal solution because a user has access to properties of modules that might not be included in the final dataset but on the other hand, everything is strongly typed. Hence, I decided not to use Dynamic object as it exposes its properties only during run-time.

## Crawler options

All adjustable settings of the crawler can be set up by creating a [CrawlerOptions](./SmartCrawler/CrawlerOptions.cs) and passing it to the `Crawler` object. It is a simple way for a user to alter the setup of the crawler according to his needs.

## Exports

I used a factory design to build an independent export library. There are only two options right now, sql and json, but anything else can be added easily. [Export Base](./SmartCrawler/Exports/ExportBase.cs) describes the methods that every Export type should contain. [Export Factory](./SmartCrawler/Exports/ExportFactory.cs) is specifically built for the crawler and returns a specific export functionality.

### SQL Export

SQL export is built from the ground up. It uses reflection to find all dataset properties and recursively goes through all of them. Then it creates an SQL code that builds a signle table. Finally, it generates all insertions to the table.

## Tests

Around 80% of the code is covered by unit tests. All methods that contain nontrivial logic must be tested thoroughly. Tests are located in the [Tests](./Tests/) project.

## Custom Html Parser

can be used by calling `Document.ParseHtml(html);`

## Used technologies from the Advanced C# course

- Locks, System.Threading.Monitor are being used in:
  - Async data structures [folder](./SmartCrawler/Structures/)
  - Best example: [AsyncStorage](./SmartCrawler/Structures/AsyncStorage.cs)
- Tasks are being used in:
  - [Crawler.cs](./SmartCrawler/Crawler.cs)
- Extension methods
  - [UriExtensionMethod](./SmartCrawler/UriExtensionMethods.cs)
  - Modules
    - Each module contains a SmartCrawler extension method that adds the module to the internal crawler list.
- Lambda functions
  - Lambda functions are used to dynamically add scraped data to the `DatasetiItem`.
  ```C#
   public Action<string, DatasetItem> Setup()
    {
        return (string html, DatasetItem item) => item.Contacts = Process(html);
    }
  ```
  Lambda functions are then executed in `SetupModules` method in [Crawler.cs](./SmartCrawler/Crawler.cs)
  - Reflection
    - Used in SQL and CSV exports
