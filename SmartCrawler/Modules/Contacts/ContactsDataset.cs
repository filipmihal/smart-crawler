namespace SmartCrawler.Modules.Contacts;

public class ContactsDataset
{
    public string[] Emails { get; }
    public string[] PhoneNumbers { get; }

    // SOCIAL OPTIONAL PROPERTIES
    public string[]? LinkedIns { get; init; } = null;
    public string[]? Facebooks { get; init; } = null;

    public ContactsDataset(string[] emails, string[] phoneNumbers)
    {
        Emails = emails;
        PhoneNumbers = phoneNumbers;
    }
}