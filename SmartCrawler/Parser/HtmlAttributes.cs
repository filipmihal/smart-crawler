namespace SmartCrawler.Parser;

public class HtmlAttributes
{
    private readonly Dictionary<string, string> _attributes = new Dictionary<string, string>();

    public void Add(string attributeName, string attributeValue)
    {
        string value;
        _attributes.TryGetValue(attributeName, out value);
        if (value is not null)
        {
            _attributes[attributeName] = value + " " + attributeValue;
            return;
        }
        _attributes.Add(attributeName, attributeValue);
    }

    public string Get(string attributeName)
    {
        string value;
        _attributes.TryGetValue(attributeName, out value);

        return value ?? "";
    }

    public List<string> GetAvailableAttributes()
    {
        return new List<string>(_attributes.Keys);
    }
}