namespace SmartCrawler.Modules;


public interface IBaseModule
{
    
    public Action<string, DatasetItem> Setup();
}

public interface IBaseModuleSetup<out T> : IBaseModule
{
    public T Process(string html);
}