using System.Text.RegularExpressions;

namespace SmartCrawler.Modules.CryptoWallets;

public class CryptoWalletsModule : IBaseModuleSetup<CryptoWalletsDataset>
{
    
    public string[] MatchBitcoinWallets(string text)
    {
        Regex pnRegex = new Regex(@"(bc1|[13])[a-zA-HJ-NP-Z0-9]{25,39}");
        var matches = pnRegex.Matches(text);
        return matches.Select(a => a.Value).ToArray().Distinct().ToArray();
    }
    
    public CryptoWalletsDataset Process(string html)
    {
        string[] bitcoinWallets = MatchBitcoinWallets(html);
        return new CryptoWalletsDataset(bitcoinWallets);
    }

    public Action<string, DatasetItem> Setup()
    {
        return (string html, DatasetItem item) => { item.Wallets = Process(html);};
    }
}

public static class CrawlerExtensionContacts
{

    public static Crawler CryptoWallets(this Crawler crawler)
    {
        return crawler.Rebuild(new CryptoWalletsModule());
    }
}