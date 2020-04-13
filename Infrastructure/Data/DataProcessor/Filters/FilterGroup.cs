using System.Collections.Generic;

namespace Exelor.Infrastructure.Data.DataProcessor.Filters
{
    public class FilterGroup : IFilter
    {
        public FilterOperator Operator { get; set; }

        public ICollection<Filter> Filters { get; set; }
        public ICollection<FilterGroup> FilterGroups { get; set; }
    }
}