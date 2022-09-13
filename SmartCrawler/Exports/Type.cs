namespace SmartCrawler.Exports;


public enum Type
{
    Json,
    Txt,
}

public static class Type2Extension
{
    public static string MatchExtension(Type type)
    {
        switch (type)
        {
            case Type.Json:
                return "json";
            case Type.Txt:
                return "txt";
            default:
                throw new ExtensionTypeNotFoundException();
        }
    }
}

public class ExtensionTypeNotFoundException : Exception{}