namespace SmartCrawler;

public class AsyncQueue<T>
{
    private readonly Queue<T> _queue = new Queue<T>();
    private readonly int _size;
    public bool Finished = false;

    public AsyncQueue(int size)
    {
        _size = size;
    }

    public void Enqueue(T elem)
    {
        lock (_queue)
        {
            if (_queue.Count >= _size)
            {
                Monitor.Wait(_queue);
            }
            
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
            Finished = true;
            Monitor.PulseAll(_queue);
        }
    }

    public T Dequeue()
    {
        lock (_queue)
        {

            while (_queue.Count <= 0)
            {
                if (Finished)
                {
                    throw new EndedQueueException();
                }
                Monitor.Wait(_queue);
            }

            T elem = _queue.Dequeue();
            
            if (_queue.Count == _size - 1)
            {
                Monitor.Pulse(_queue);
            }
            return elem;
        }
        
    }
        
    public class EndedQueueException : Exception {}
}