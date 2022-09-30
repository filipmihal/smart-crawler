using System.Text.RegularExpressions;

namespace SmartCrawler.Modules.Contacts;

public class ContactsModule : IBaseModuleSetup<ContactsDataset>
{
    private bool _scrapeSocial;

    public ContactsModule(bool scrapeSocial)
    {
        _scrapeSocial = scrapeSocial;
    }

    public string[] MatchEmails(string text)
    {
        // Matches emails according to official standard
        // Emails that are present in Javascript will be matched as well
        Regex emailRegex = new Regex("([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5})");
        var matches = emailRegex.Matches(text);
        return matches.Select(a => a.Value).Distinct().ToArray();
    }

    public string[] MatchPhoneNumbers(string text)
    {
        // Matches international phone numbers with different prefixes
        // There is some margin of error present when it comes to matching phone numbers that do not contain dashes, plus or brackets
        Regex pnRegex = new Regex("((\\+\\d{1,3}\\s)?\\(?\\d{3}\\)?[\\s.-]\\d{3}[\\s.-]\\d{3,4})|(\\+\\d{10,13})");
        var matches = pnRegex.Matches(text);
        return matches.Select(a => a.Value).Distinct().ToArray();
    }

    public string[] MatchFacebooks(string text)
    {
        Regex pnRegex = new Regex(@"(?:http(?:s)?:\/\/)?(?:www\.)?(?:facebook\.com|fb\.com)\/[a-z|\/|0-9|*'\.();\-:@&=+$,/?%#]*");
        var matches = pnRegex.Matches(text);
        return matches.Select(a => a.Value).Distinct().ToArray();
    }

    public string[] MatchLinkedIns(string text)
    {
        Regex pnRegex = new Regex(@"(?:http(?:s)?:\/\/)?(?:www\.)?(?:linkedin\.com)\/[a-z|\/|0-9|\.*'();\-:@&=+$,/?%#]*");
        var matches = pnRegex.Matches(text);
        return matches.Select(a => a.Value).Distinct().ToArray();
    }

    public ContactsDataset Process(string html)
    {
        var emails = MatchEmails(html);
        var phoneNumbers = MatchPhoneNumbers(html);
        if (_scrapeSocial)
        {
            var linkedIns = MatchLinkedIns(html);
            var facebooks = MatchFacebooks(html);
            return new ContactsDataset(emails, phoneNumbers) { LinkedIns = linkedIns, Facebooks = facebooks };
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
    public static Crawler<T> Contacts<T>(this Crawler<T> crawler, bool social) where T : DatasetItem, new()
    {
        return crawler.Rebuild(new ContactsModule(social));
    }
}