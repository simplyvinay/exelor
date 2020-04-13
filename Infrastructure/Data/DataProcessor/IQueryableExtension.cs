using System.Linq;
using Exelor.Infrastructure.Data.DataProcessor.Filters;
using Exelor.Infrastructure.Data.DataProcessor.Mapping;
using LinqKit;

namespace Exelor.Infrastructure.Data.DataProcessor
{
    public static class IQueryableExtension
    {
        public static SearchResult<TEntityType> Apply<TEntityType>(
            this IQueryable<TEntityType> query,
            FilterSet filter,
            MappingCollection<TEntityType> mappings
        ) where TEntityType : class
        {
            if (filter.PageNumber < 1) filter.PageNumber = 1;

            if (filter.HasFilters())
            {
                query = query.AsExpandable().Where(
                    SearchHelper.CreateSearchPredicate(
                        filter,
                        mappings)
                );
            }

            query = ApplySort(
                query,
                filter,
                mappings);

            var resIdx = (filter.PageSize * filter.PageNumber) - filter.PageSize;

            return new SearchResult<TEntityType>
            {
                TotalResults = query.Count(),
                Results = query.Skip(resIdx)
                    .Take(filter.PageSize)
                    .ToList()
            };
        }

        private static IQueryable<TEntityType> ApplySort<TEntityType>(
            IQueryable<TEntityType> query,
            FilterSet filter,
            MappingCollection<TEntityType> mappings
        ) where TEntityType : class
        {
            var isDescending = filter.SortDir == "desc";
            var isAscending = filter.SortDir == "asc";

            bool? selectedSort = isDescending ? true : (isAscending ? false : (bool?) null);

            if (!string.IsNullOrWhiteSpace(filter.SortBy))
            {
                var mapping = mappings.GetMapping(filter.SortBy);

                return mapping.ApplySort(
                    query,
                    selectedSort ?? mappings.DefaultSort.Descending);
            }

            var defaultSort = mappings.DefaultSort;
            return defaultSort.Mapping.ApplySort(
                query,
                selectedSort ?? mappings.DefaultSort.Descending);
        }
    }
}