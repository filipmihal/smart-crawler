using SmartCrawler;

namespace Tests;

class TestItem : IAsyncQueueItem
{
    public string Name;

    public TestItem(string name)
    {
        Name = name;
    }

    public string GetKey()
    {
        return Name;
    }
}

public class AsyncUniqueQueueTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestUniqueness()
    {
        TestItem a = new TestItem("hello");
        TestItem b = new TestItem("hello2");
        TestItem aCopy = new TestItem("hello");

        AsyncUniqueQueue<TestItem> queue = new AsyncUniqueQueue<TestItem>();
        queue.Enqueue(a);
        queue.Enqueue(b);
        queue.Enqueue(aCopy);
        queue.Enqueue(b);
        
        Assert.That(queue.Dequeue(), Is.EqualTo(a));
        Assert.That(queue.Dequeue(), Is.EqualTo(b));
        Assert.Throws<InvalidOperationException>(() => queue.Dequeue());

    }
}