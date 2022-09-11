using SmartCrawler.Modules.Contacts;
using SmartCrawler.Modules.CryptoWallets;

namespace SmartCrawler.Modules;

public class DatasetItem
{
    public string Url { get; set; }
    public string[]? SubUrls { get; set; }
    public int? Depth { get; set; }
    public ContactsDataset? Contacts { get; set; }
    public CryptoWalletsDataset Wallets { get; set; }

    public DatasetItem(string url)
    {
        Url = url;
    }
}