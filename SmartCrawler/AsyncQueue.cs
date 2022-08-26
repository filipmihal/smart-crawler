namespace SmartCrawler;

public class AsyncQueue<T>
{
    private readonly Queue<T> _queue;

    public AsyncQueue()
    {
        _queue = new Queue<T>();
    }

    public AsyncQueue(T[] initialArray)
    {
        _queue = new Queue<T>(initialArray);
    }

    public void Enqueue(T elem)
    {
        lock (_queue)
        {
            _queue.Enqueue(elem);
            
            if (_queue.Count == 1)
            {
                Monitor.PulseAll(_queue);
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
        
    public class EndedQueueException : Exception {}
}