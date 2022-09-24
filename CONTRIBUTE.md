# Contribute (Programming documentation)

When developing SmartCrawler, my main focus was on the speed of the cralwer and on providing the best developer experience for those who will use it. User (developer) should be able to spend max 15 minutes reading the documentation and have a working crawler ready in max <!-- TODO: number --> simple steps. Additionally, the scraper shall not only be used on simple crawling tasks but should be easily scalable and serve as a base for complex software projects.

## Scraping on the low level

To get the content from online resources, SmartCrawler uses `HttpClient` library. It is a built-in DOTNET library that makes http requests to given URLs and provides raw response body. The reason I chose this library is that it is fast, easy to use and thread-safe. To improve the user experience I built a wrapper around HttpClient called [HtmlScraper](./SmartCrawler/HtmlScraper.cs). Throught the whole crawling process, we use a single HttpClient (as is recommended in the official documentation) in order to not cause port exhaustion.

### Webpage rendering

Right now, SmartCrawler does not support rendering of scraped websites. That might lead to unwanted data loss. Arguably, most of the websites that provide interesting data for scraping are static websites or use SSR, so using an http based scraping is a convenient solution. In the future, we might implement a webpage rendering functionality to support a wider range of possibly scraped websites. However, it will rapidly increase the crawling time.

## Thread-safe data structures

The essential feature of this project is parallelism. Hence, it is quite intuitive that we have to implement thread-safe data structures in order to avoid any race data, race conditions or dead locks. SmartCrawler uses multiple async datastructures. They are located in [Structures](./SmartCrawler/Structures/) folder.

- [AsyncUniqueQueue](./SmartCrawler/Structures/AsyncUniqueQueue.cs)
  - It is a thread-safe queue
  - Additionally, it uses an internal hashset to store items that were already enqued.
  - The main idea is to only enqueue unique items
  - SmartCrawler uses it to store all URLs that are needed to be crawled. The data structure is built in a way that every URL will be scraped once and crawler will avoid any loops. This saves time and keeps the final dataset in a clean state.
- [AsyncStorage](./SmartCrawler/Structures/AsyncStorage.cs)
  - Simple thread-safe storage
  - SmartCrawler uses it to save scraped data
- [ThreadState](./SmartCrawler/Structures/ThreadState.cs)
  - Stores states of all threads that are being used and provides a simple APIto access states of all threads simultaneously
  - It is an extremely important part of the project. Its purpose is going to be explained later in the text.

## Parallel Crawling

In order to run multiple crawlers in parallel, we use `System.Threading.Tasks`. The logic for parallel crawling is included in the main [Crawler.cs](./SmartCrawler/Crawler.cs) file. Basically, we create numerous independent tasks and then use `Task.WhenAll` to execute them. All shared data structures that are used by the crawling tasks must be thread-safe. The logic of a single crawler is located in `Crawl` method. `StartAsync` creates and runs all the tasks.

### Thread state management

One issue that arises when using parallel crawlers is that crawlers' states are dependent of each other. A single crawler can stop crawling only if itself and every other crawler is in the finished state. Otherwise there might come a new batch of URLs that should be processed.
Therefore, I implemented [ThreadState data structure](./SmartCrawler/Structures/ThreadState.cs) in the `Crawl` method.

## Modules

The most distinctive feature of SmartCrawler is its Modules system. It was arguably the the most challenging engineering task of the project. The main idea

- variety of modules that user can add to his crawler
- these modules process the raw html response from the HtmlScraper and update the dataset accordingly
- Final dataset has to have a strongly typed nature
- Firstly, I was thinking about using dynamic object but it exposes its properties only during run-time which is not ideal when user wants to explore the scraped dataset and use tools such as Linq.
  Modules have their base tructure TODO
- Base dataset interface
- They should be independent of the main crawler. Hence they use extension methods to update Crawler object.
- caveat ... final dataset contains all modules dataset. However, the datasets of unused modules are always null.
- The good thing is that we will never reach duplicit propery naming.

## Exports

- Base
- idea behind it
- factory
- Custom SQL that uses reflection

## Crawler options

All adjustable settings of the crawler can be set up by creating a [CrawlerOptions](./SmartCrawler/CrawlerOptions.cs) and passing it to the `Crawler` object. It is a simple way for a user to alter the setup of the crawler according to his needs.

## Used technologies from the Advanced C# course

- Locks, System.Threading.Monitor are being used in:
  - Async data structures [folder](./SmartCrawler/Structures/)
  - Best example: [AsyncStorage](./SmartCrawler/Structures/AsyncStorage.cs)
- Tasks are being used in:
  - [Crawler.cs](./SmartCrawler/Crawler.cs)
- Extension methods
  - [UriExtensionMethod](./SmartCrawler/UriExtensionMethods.cs) TODO
  - Modules TODO
- Lambda functions
  - Modules TODO
