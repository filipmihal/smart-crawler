namespace SmartCrawler;

public interface IAsyncQueueItem
{
    public string GetKey();
}

public class AsyncUniqueQueue<T> where T : IAsyncQueueItem

{
    private readonly Queue<T> _queue = new Queue<T>();
    private readonly HashSet<string> _set = new HashSet<string>();

    public AsyncUniqueQueue()
    {
        _queue = new Queue<T>();
    }

    public AsyncUniqueQueue(T[] initialArray)
    {
        EnqueueList(initialArray);
    }

    public void Enqueue(T elem)
    {
        lock (_queue)
        {
            if (_set.Contains(elem.GetKey()))
            {
                return;
            }

            _set.Add(elem.GetKey());
            _queue.Enqueue(elem);

            if (_queue.Count == 1)
            {
                Monitor.PulseAll(_queue);
            }
        }
    }

    public void EnqueueList(IEnumerable<T> elems)
    {
        lock (_queue)
        {
            foreach (T elem in elems)
            {
                if (_set.Contains(elem.GetKey()))
                {
                    continue;
                }
                
                _set.Add(elem.GetKey());
                _queue.Enqueue(elem);

                if (_queue.Count == 1)
                {
                    Monitor.PulseAll(_queue);
                }

            }
        }
    }

    public void Finish()
    {
        lock (_queue)
        {
            Monitor.PulseAll(_queue);
        }
    }

    public T Dequeue()
    {
        lock (_queue)
        {
            T elem = _queue.Dequeue();
            return elem;
        }

    }

    public class EndedQueueException : Exception { }
}