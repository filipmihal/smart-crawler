using SmartCrawler.Modules;

namespace SmartCrawler.Exports;



public abstract class ExportBase<T> 
{
    protected ExportOptions ExportOptions { get; }

    public ExportBase(ExportOptions exportOptions)
    {
        ExportOptions = exportOptions;
    }

    public abstract string GetExtension();

    public abstract void Export(List<T> items);
}