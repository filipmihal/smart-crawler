using SmartCrawler.Modules;

namespace SmartCrawler.Exports;

public abstract class Base
{
    protected Options Options { get; set; }

    protected Base(Options options)
    {
        Options = options;
    }

    public virtual void Export(List<DatasetItem> items){}
}