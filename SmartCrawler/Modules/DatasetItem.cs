using SmartCrawler.Modules.Contacts;
using SmartCrawler.Modules.Images;

namespace SmartCrawler.Modules;

public class DatasetItem
{
    public string Url;
    public string[]? SubUrls;
    public int? Depth;
    public ContactsDataset? Contacts;

    public DatasetItem(string url)
    {
        Url = url;
    }
}