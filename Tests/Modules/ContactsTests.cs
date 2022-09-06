using SmartCrawler.Modules.Contacts;

namespace Tests.Modules;

public class ContactsTests
{
    ContactsModule _module = new ContactsModule();

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
            </head>
            petr.fekete@gmail.io
            <body>
            </body>
            <script>@length.i
       </html>";
        string[] expectedArray = new[] { "filip@cuni.cz", "petr.fekete@gmail.io" };
        string[] actualEmails = _module.MatchEmails(textString);
        CollectionAssert.AreEqual(expectedArray, actualEmails);

    }

    [Test]
    public void TestInvalidEmailString()
    {
        string invalidString = "@ .conf @. ,. f.r@?.com  k@jojjo . @#  ";
        string[] actualEmails = _module.MatchEmails(invalidString);
        Assert.That(actualEmails.Length, Is.EqualTo(0));

    }
    
}