using SmartCrawler.Modules.CryptoWallets;

namespace Tests.Modules;

public class CryptoWalletsTests
{
    private CryptoWalletsModule _module = new CryptoWalletsModule();
    
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestEmailRegex()
    {
        string textString = @"
       <html>
            <head> ""filip@cuni.cz""
            </head>nw9re59gtzzwf5mdqnw9re59gtzzwf5mdqnw9re59gtzzwf5mdqnw9re59gtzzwf5mdq
            </head>nw9re59gtzzwf5mdqnw9re
            <body>bc1qar0srrr7xfkvy5l643lydnw9re59gtzzwf5mdq
            </body>
            <script>
       </html>";
        string[] expectedArray = new[] { "bc1qar0srrr7xfkvy5l643lydnw9re59gtzzwf5mdq" };
        string[] actualEmails = _module.MatchBitcoinWallets(textString);
        CollectionAssert.AreEqual(expectedArray, actualEmails);

    }
}