namespace SmartCrawler.Exports
{
    public class Csv<T> : ExportBase<T>
    {
        private Type[] _supported = new[] {typeof(bool),typeof(string),typeof(int),typeof(decimal),typeof(float),typeof(double),typeof(DateTime),typeof(string[])};
        

        public Csv(ExportOptions exportOptions) : base(exportOptions)
        {
        }

        public override string GetExtension()
        {
            throw new NotImplementedException();
        }

        public override void Export(List<T> items)
        {
            throw new NotImplementedException();
        }
    }
}