using System.Text.RegularExpressions;

namespace SmartCrawler.Modules.Contacts;

public class ContactsModule: IBaseModuleSetup<ContactsDataset>
{
    private bool _scrapeSocial;

    public ContactsModule(bool scrapeSocial)
    {
        _scrapeSocial = scrapeSocial;
    }

    public string[] MatchEmails(string text)
    {
        Regex emailRegex = new Regex("([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5})");
        var matches = emailRegex.Matches(text);
        return matches.Select(a => a.Value).ToArray().Distinct().ToArray();
    }

    public string[] MatchPhoneNumbers(string text)
    {
        Regex pnRegex = new Regex("((\\+\\d{1,3}\\s)?\\(?\\d{3}\\)?[\\s.-]\\d{3}[\\s.-]\\d{3,4})|(\\+\\d{10,13})");
        var matches = pnRegex.Matches(text);
        return matches.Select(a => a.Value).ToArray().Distinct().ToArray();
    }

    public string[] MatchFacebooks(string text)
    {
        Regex pnRegex = new Regex(@"(?:http(?:s)?:\/\/)?(?:www\.)?(?:facebook\.com|fb\.com)\/[a-z|\/|0-9|*'\.();:@&=+$,/?%#]*");
        var matches = pnRegex.Matches(text);
        return matches.Select(a => a.Value).ToArray().Distinct().ToArray();
    }
    
    public string[] MatchLinkedIns(string text)
    {
        Regex pnRegex = new Regex(@"(?:http(?:s)?:\/\/)?(?:www\.)?(?:linkedin\.com)\/[a-z|\/|0-9|\.*'();:@&=+$,/?%#]*");
        var matches = pnRegex.Matches(text);
        return matches.Select(a => a.Value).ToArray().Distinct().ToArray();
    }

    public ContactsDataset Process(string html)
    {
        var emails = MatchEmails(html);
        var phoneNumbers = MatchPhoneNumbers(html);
        if (_scrapeSocial)
        {
            var linkedIns = MatchLinkedIns(html);
            var facebooks = MatchFacebooks(html);
            return new ContactsDataset(emails, phoneNumbers, linkedIns, facebooks);
        }
        
        ContactsDataset contacts = new ContactsDataset(emails, phoneNumbers);

        return contacts;
    }

    public Action<string, DatasetItem> Setup()
    {
        return (string html, DatasetItem item) => item.Contacts = Process(html);
    }
}

public static class CrawlerExtensionContacts
{

    public static Crawler Contacts(this Crawler crawler, bool social)
    {
        return crawler.Rebuild(new ContactsModule(social));
    }
}