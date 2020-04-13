namespace Exelor.Infrastructure.Data.DataProcessor.Filters
{
    public class Filter : IFilter
    {
        public string Field { get; set; }
        public string Value { get; set; }
        public DataOperator Action { get; set; }
    }
}