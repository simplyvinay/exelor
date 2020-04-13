using System;
using System.Linq;
using System.Linq.Expressions;

namespace Exelor.Infrastructure.Data.DataProcessor.Mapping
{
    public interface IFilterMap<TEntityType> where TEntityType : class
    {
        bool IsDefaultSortFilter { get; }
        string Field { get; }

        IQueryable<TEntityType> ApplySort(
            IQueryable<TEntityType> query,
            bool descending);

        Expression<Func<TEntityType, bool>> GetFilterLambda(
            string value,
            DataOperator dataOperator);
    }
}