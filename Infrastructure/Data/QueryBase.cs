using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Exelor.Infrastructure.Data.DataProcessor;
using Exelor.Infrastructure.Data.DataProcessor.Filters;
using Exelor.Infrastructure.Data.DataProcessor.Mapping;

namespace Exelor.Infrastructure.Data
{
    public abstract class QueryBase<T> where T : Entity
    {
        private FilterSet _filterSet;
        private const string EscapedCommaPattern = @"(?<!($|[^\\])(\\\\)*?\\),";
        private readonly string[] Operators = new string[]
        {
            "=",
            "!=",
            ">=",
            "<=",
            ">",
            "<"
        };
        
        //"name=role,id>=1"
        protected QueryBase(
            int pageNumber,
            int pageSize,
            string sortBy,
            string sortDirection,
            string filterParams)
        {
            BuildFilterSet(
                pageNumber,
                pageSize,
                sortBy,
                sortDirection,
                filterParams);
        }

        private void BuildFilterSet(
            int pageNumber,
            int pageSize,
            string sortBy,
            string sortDirection,
            string filterParams)
        {
            _filterSet = new FilterSet
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDir = sortDirection,
                Filter = new FilterGroup
                {
                    Operator = FilterOperator.Or,
                    Filters = new List<Filter>(),
                    FilterGroups = new List<FilterGroup>()
                }
            };


            if (!string.IsNullOrEmpty(filterParams))
            {
                foreach (var filter in Regex.Split(
                    filterParams,
                    EscapedCommaPattern))
                {
                    var filterSplits = filter.Split(
                            Operators,
                            StringSplitOptions.RemoveEmptyEntries)
                        .Select(t => t.Trim()).ToArray();
                    var name = filterSplits[0];
                    var value = filterSplits[1];
                    var oper = Array.Find(
                                   Operators,
                                   o => filter.Contains(o)) ?? "=";

                    _filterSet.Filter.Filters.Add(
                        new Filter
                        {
                            Action = GetOperator(oper),
                            Field = name,
                            Value = value
                        });
                }
            }
        }

        private DataOperator GetOperator(
            string oper)
        {
            return oper switch
            {
                "=" => DataOperator.Eq,
                ">" => DataOperator.Gt,
                ">=" => DataOperator.GtEq,
                "<" => DataOperator.Lt,
                "<=" => DataOperator.LtEq,
                _ => DataOperator.Like
            };
        }

        public FilterSet FilterSet => _filterSet;
        public abstract MappingCollection<T> GetMappings();

    }
}