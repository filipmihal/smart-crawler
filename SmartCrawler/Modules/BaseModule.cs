namespace SmartCrawler.Modules;

/**
* Base interface for all modules
* Each module must be inherited from this interface
*/
public interface IBaseModule
{
    /// <summary>
    ///  Matches a DatasetItem property to the result of the processing function
    /// </summary>
    /// <remarks>
    /// Returns an action the matches a property instead of matching it right away. 
    /// That is done because all actions are then ran together in the crawler.
    /// </remarks>
    public Action<string, DatasetItem> Setup();
}

public interface IBaseModuleSetup<out T> : IBaseModule
{
    /// <summary>
    ///  Processes the raw html content and returns the data in a form of a pre-set module data type
    /// </summary>
    /// <remarks>
    /// Most of the processing logic is comprised of complex regex expressions
    /// </remarks>
    public T Process(string html);
}