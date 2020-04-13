using System.Collections.Generic;
using System.Linq;

namespace Exelor.Infrastructure.Data.DataProcessor.Filters
{
    public class FilterSet
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SortDir { get; set; }
        public FilterGroup Filter { get; set; }

        public void AddRequiredFilterGroup(
            FilterGroup filterGroup)
        {
            if (Filter.Operator == FilterOperator.And)
            {
                this.Filter.FilterGroups.Add(filterGroup);
            }
            else
            {
                Filter = new FilterGroup
                {
                    Operator = FilterOperator.And,
                    Filters = new List<Filter>(),
                    FilterGroups = new[]
                    {
                        new FilterGroup
                        {
                            Filters = Filter.Filters,
                            FilterGroups = Filter.FilterGroups,
                            Operator = Filter.Operator
                        },
                        filterGroup
                    }
                };
            }
        }

        public void AddRequiredFilter(
            string field,
            string value)
        {
            if (Filter.Operator == FilterOperator.And)
            {
                this.Filter.Filters.Add(
                    new Filter
                    {
                        Action = DataOperator.Eq,
                        Field = field,
                        Value = value
                    });
            }
            else
            {
                Filter = new FilterGroup
                {
                    Operator = FilterOperator.And,
                    Filters = new List<Filter>
                    {
                        new Filter
                        {
                            Action = DataOperator.Eq,
                            Field = field,
                            Value = value
                        }
                    },
                    FilterGroups = new[]
                    {
                        new FilterGroup
                        {
                            Filters = Filter.Filters,
                            FilterGroups = Filter.FilterGroups,
                            Operator = Filter.Operator
                        }
                    }
                };
            }
        }

        public bool HasFilters()
        {
            return Filter != null && ((Filter.Filters != null && Filter.Filters.Any()) ||
                                      (Filter.FilterGroups != null && Filter.FilterGroups.Any()));
        }
    }
}
