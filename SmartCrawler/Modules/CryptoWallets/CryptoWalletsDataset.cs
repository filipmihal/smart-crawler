namespace SmartCrawler.Modules.CryptoWallets;

public class CryptoWalletsDataset
{
    public string[] Bitcoin { get; }

    public CryptoWalletsDataset(string[] bitcoin)
    {
        Bitcoin = bitcoin;
    }
}