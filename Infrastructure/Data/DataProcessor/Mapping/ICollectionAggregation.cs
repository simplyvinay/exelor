using System;
using System.Linq.Expressions;

namespace Exelor.Infrastructure.Data.DataProcessor.Mapping
{
    public interface ICollectionAggregation<TRootEntityType, TCollectionEntityType>
        where TCollectionEntityType : class
    {
        Expression<Func<TRootEntityType, bool>> GetPredicate<TCollectionSubType>(
            Expression<Func<TCollectionEntityType, bool>> methodPredicate
        );

        string GetField();
    }
}