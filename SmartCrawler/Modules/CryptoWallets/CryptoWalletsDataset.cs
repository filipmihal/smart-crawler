namespace SmartCrawler.Modules.CryptoWallets;

public struct CryptoWalletsDataset
{
    public string[] Bitcoin { get; }

    public CryptoWalletsDataset(string[] bitcoin)
    {
        Bitcoin = bitcoin;
    }
}