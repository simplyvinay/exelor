using System;
using System.Collections.Generic;
using System.Linq;

namespace Exelor.Infrastructure.Data.DataProcessor.Mapping
{
    public class MappingCollection<TEntityType>
        where TEntityType : class
    {
        private readonly IEnumerable<IFilterMap<TEntityType>> mappings;
        private string defaultMapField;
        private bool defaultSortDescending;

        public MappingCollection(
            IEnumerable<IFilterMap<TEntityType>> mappings)
        {
            this.mappings = mappings;
        }

        internal DefaultSort<TEntityType> DefaultSort
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.defaultMapField))
                {
                    return new DefaultSort<TEntityType>(
                        mappings.First(m => m.Field == defaultMapField),
                        defaultSortDescending
                    );
                }

                var defaultSort = mappings
                    .FirstOrDefault(m => m.IsDefaultSortFilter);

                if (defaultSort != null)
                {
                    return new DefaultSort<TEntityType>(
                        defaultSort,
                        defaultSortDescending);
                }

                if (mappings.Count() > 0)
                {
                    return new DefaultSort<TEntityType>(
                        mappings.First(),
                        defaultSortDescending);
                }

                throw new Exception("No default mapping found");
            }
        }

        internal IFilterMap<TEntityType> GetMapping(
            string field)
        {
            var mapping = mappings.FirstOrDefault(m => m.Field == field);

            if (mapping == null)
            {
                // todo throw exception so consumer knows their filters aren't being applied
                throw new Exception($"{field} is not known");
            }

            return mapping;
        }

        public MappingCollection<TEntityType> WithDefaultSort(
            string field,
            bool isDescending)
        {
            if (mappings.All(m => m.Field == field))
            {
                throw new Exception($"No mapping found for {field}");
            }

            defaultMapField = field;
            defaultSortDescending = isDescending;
            return this;
        }
    }

    internal class DefaultSort<TEntityType>
        where TEntityType : class
    {
        public DefaultSort(
            IFilterMap<TEntityType> mapping,
            bool descending)
        {
            Mapping = mapping;
            Descending = descending;
        }

        public IFilterMap<TEntityType> Mapping { get; }
        public bool Descending { get; }
    }
}