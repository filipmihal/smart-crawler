namespace SmartCrawler.Modules.Contacts;

public class ContactsModule: IBaseModuleSetup<ContactsDataset>
{
    public ContactsDataset Process(string html)
    {
        
        // TODO: implement real contact scraper
        ContactsDataset contacts = new ContactsDataset(new []{""}, new []{""}, new []{""});

        return contacts;
    }

    public Action<string, DatasetItem> Setup()
    {
        return (string html, DatasetItem item) => item.Contacts = Process(html);
    }
}

public static class CrawlerExtensionContacts
{

    public static Crawler Contacts(this Crawler crawler)
    {
        return crawler.Rebuild(new ContactsModule());
    }
}