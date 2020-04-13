using System;
using System.Linq;
using System.Linq.Expressions;

namespace Exelor.Infrastructure.Data.DataProcessor.Mapping
{
    public class FilterMapping<TEntityType, TPropType>
        : IFilterMap<TEntityType>
        where TEntityType : class
    {
        public FilterMapping(
            string field,
            Expression<Func<TEntityType, TPropType>> mapping)
        {
            Field = field;
            Mapping = mapping;
        }

        public string Field { get; set; }

        public bool IsDefaultSortFilter { get; set; } = false;
        private Expression<Func<TEntityType, TPropType>> Mapping { get; set; }

        public IQueryable<TEntityType> ApplySort(
            IQueryable<TEntityType> query,
            bool descending)
        {
            return query.OrderByWithDirection(
                Mapping,
                descending);
        }

        public Expression<Func<TEntityType, bool>> GetFilterLambda(
            string value,
            DataOperator dataOperator)
        {
            return SearchHelper.GetLambdaExpression<TEntityType, TPropType>(
                Mapping,
                value,
                dataOperator);
        }
    }
}