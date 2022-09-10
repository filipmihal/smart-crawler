namespace SmartCrawler.Modules.CryptoWallets;

public struct CryptoWalletsDataset
{
    public readonly string[] Bitcoin;

    public CryptoWalletsDataset(string[] bitcoin)
    {
        Bitcoin = bitcoin;
    }
}