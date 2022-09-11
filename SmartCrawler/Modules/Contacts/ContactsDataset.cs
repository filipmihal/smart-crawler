namespace SmartCrawler.Modules.Contacts;

public struct ContactsDataset
{
    public string[] Emails { get; }
    private string[] PhoneNumbers { get; }
    public string[]? LinkedIns { get; init; } = null;
    public string[]? Facebooks { get; init; } = null;

    public ContactsDataset(string[] emails, string[] phoneNumbers)
    {
        Emails = emails;
        PhoneNumbers = phoneNumbers;
    }
}