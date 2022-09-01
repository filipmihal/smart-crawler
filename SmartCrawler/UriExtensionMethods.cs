namespace SmartCrawler;

public static class UriExtensionMethods
{
  
    public static string CleanHost(this Uri uri)
    {
        string[] names = uri.Host.Split('.');
        if (names.Length > 1)
        {
            int t = names.Length;
            return names[t - 2];
        }

        return names[0];
    }
    

}