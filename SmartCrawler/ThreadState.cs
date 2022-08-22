namespace SmartCrawler;

public class ThreadState
{
    private bool[] _finishedStates;
    
    public ThreadState(int numberOfThreads)
    {
        _finishedStates = new bool[numberOfThreads];
    }

    public bool AllFinished()
    {
        bool areFinished = false;
        lock (_finishedStates)
        {
            areFinished = _finishedStates.All(x => x);
        }
        return areFinished;
    }

    public void UpdateState(int threadId, bool newFinishedState)
    {
        lock (_finishedStates)
        {
            _finishedStates[threadId] = newFinishedState;
        }
    }

    public bool UpdateAndCanFinish(int threadId, bool newState)
    {
        bool areFinished = false;
        lock (_finishedStates)
        {
            _finishedStates[threadId] = newState;
            areFinished = _finishedStates.All(x => x);
        }

        return areFinished;
    }
}