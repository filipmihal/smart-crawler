namespace SmartCrawler.Modules.Contacts;

public struct ContactsDataset
{
    public readonly string[] Emails;
    public readonly string[] PhoneNumbers;
    public readonly string[] LinkedIns;

    public ContactsDataset(string[] emails, string[] phoneNumbers, string[] linkedIns)
    {
        Emails = emails;
        PhoneNumbers = phoneNumbers;
        LinkedIns = linkedIns;
    }
}