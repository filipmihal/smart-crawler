namespace SmartCrawler.Modules.Contacts;

public struct ContactsDataset
{
    public readonly string[] Emails;
    public readonly string[] PhoneNumbers;
    public readonly string[] LinkedIns;
    public readonly string[] Facebooks;

    public ContactsDataset(string[] emails, string[] phoneNumbers, string[] linkedIns, string[] facebooks)
    {
        Emails = emails;
        PhoneNumbers = phoneNumbers;
        LinkedIns = linkedIns;
        Facebooks = facebooks;
    }
}