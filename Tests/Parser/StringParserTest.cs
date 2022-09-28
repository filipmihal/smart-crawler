

using System.Text.RegularExpressions;
using SmartCrawler.Parser;

namespace Tests.Parser;

public class StringParserTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestReadAll()
    {
        string testString1 = "asdf123";
        string testString2 = "asdf 123";
        string testString3 = "/asdf 123";

        StringParser testParser1 = new StringParser(testString1);
        StringParser testParser2 = new StringParser(testString2);
        StringParser testParser3 = new StringParser(testString3);

        Regex reg = new Regex("[0-9]|[a-z]|[A-Z]");
        Assert.That(testParser1.ReadAll(reg), Is.EqualTo("asdf123"));
        Assert.That(testParser2.ReadAll(reg), Is.EqualTo("asdf"));
        Assert.That(testParser3.ReadAll(reg), Is.EqualTo(""));
    }
}
