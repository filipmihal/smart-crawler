using SmartCrawler.Modules.Contacts;

namespace Tests.Modules;

public class ContactsTests
{
    ContactsModule _module = new ContactsModule(scrapeSocial: true);

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
    
    [Test]
    public void TestPhoneNumbersRegex()
    {
        string textString = @"
       <html>
            <head> +420919086788
            </head>
            petr.fekete@gmail.io
            <body>
            </body>123-456-7890
            <script>@length.i
            +21 (123) 456-7890
       </html>";
        string[] expectedArray = new[] { "+420919086788", "123-456-7890", "+21 (123) 456-7890" };
        string[] actualPn = _module.MatchPhoneNumbers(textString);
        CollectionAssert.AreEqual(expectedArray, actualPn);
    }
    
    [Test]
    public void TestInvalidPhoneNumbersRegex()
    {
        string textString = @"
       <html>
            <head> +420919086
            </head>
            petr.fekete@gmail.io
            <body>
            </body>123-456-7
            <script>@length.i
            +21 +0 0
        1999/99/99
           1234567890 
       </html>";
        string[] actualPn = _module.MatchPhoneNumbers(textString);
        Assert.That(actualPn.Length, Is.EqualTo(0));
    }
    [Test]
    public void TestFacebooksRegex()
    {
        string textString = @"
       <html>
https://www.facebook.com/ahoj/ppp

www.fb.com/test#title
       </html>";
        string[] expectedArray = new[] { "https://www.facebook.com/ahoj/ppp", "www.fb.com/test#title" };
        string[] actualPn = _module.MatchFacebooks(textString);
        CollectionAssert.AreEqual(expectedArray, actualPn);
    }
    
}