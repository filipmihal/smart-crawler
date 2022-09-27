using SmartCrawler.Modules.Contacts;
using SmartCrawler.Modules.CryptoWallets;

namespace SmartCrawler.Modules;

/**
* Represents the final data format that is going to be used after the crawling has finished
* Each module must have its property defined here. Otherwise, the module dataset would not be accessible
*  
* Each module property must be marked as nullable because of the optional nature of every module
*/  
public class DatasetItem
{

    /// <summary>
    /// URL is a required property that identifies each item in the final list
    /// </summary>
    public string Url { get; set; }

    // These two properties are null only if the CrawlingDepthOptions is not defined or its value is set to 0
    public string[]? SubUrls { get; set; }
    public int? Depth { get; set; }


    // MODULE PROPERTIES
    // Set them as nullable
    public ContactsDataset? Contacts { get; set; }
    public CryptoWalletsDataset? Wallets { get; set; }

    public DatasetItem(string url)
    {
        Url = url;
    }
}