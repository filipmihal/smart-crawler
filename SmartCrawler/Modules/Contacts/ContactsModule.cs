using System.Text.RegularExpressions;

namespace SmartCrawler.Modules.Contacts;

public class ContactsModule: IBaseModuleSetup<ContactsDataset>
{
    public string[] MatchEmails(string text)
    {
        Regex emailRegex = new Regex("([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5})");
        var matches = emailRegex.Matches(text);
        return matches.Select(a => a.Value).ToArray();
    }

    public string[] MatchPhoneNumbers(string text)
    {
        Regex pnRegex = new Regex("((\\+\\d{1,3}\\s)?\\(?\\d{3}\\)?[\\s.-]\\d{3}[\\s.-]\\d{3,4})|(\\+\\d{10,13})");
        var matches = pnRegex.Matches(text);
        return matches.Select(a => a.Value).ToArray();
    }

    public string[] MatchFacebooks(string text)
    {
        Regex pnRegex = new Regex(@"(?:http(?:s)?:\/\/)?(?:www\.)?(?:facebook\.com|fb\.com)\/[a-z|\/|0-9|*'();:@&=+$,/?%#]*");
        var matches = pnRegex.Matches(text);
        return matches.Select(a => a.Value).ToArray();
    }

    public ContactsDataset Process(string html)
    {
       
        ContactsDataset contacts = new ContactsDataset(new []{""}, new []{""}, new []{""}, new []{""});

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