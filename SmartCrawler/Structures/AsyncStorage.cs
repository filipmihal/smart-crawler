using System.Reflection.Metadata;

namespace SmartCrawler;

/***
 * Thread-safe list
 */
public class AsyncStorage<T>
{
    private readonly List<T> _list = new List<T>();

    public void Add(T elem)
    {
        lock (_list)
        {
            _list.Add(elem);
        }
    }

    public List<T> Export()
    {
        return _list;
    }
}