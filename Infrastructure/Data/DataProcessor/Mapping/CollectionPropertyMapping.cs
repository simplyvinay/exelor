using System;
using System.Linq;
using System.Linq.Expressions;

namespace Exelor.Infrastructure.Data.DataProcessor.Mapping
{
    public class CollectionPropertyMapping<TEntityType, TCollectionEntityType, TPropertyType> : IFilterMap<TEntityType>
        where TEntityType : class
        where TCollectionEntityType : class
    {
        private readonly ICollectionAggregation<TEntityType, TCollectionEntityType> collection;
        private readonly Expression<Func<TCollectionEntityType, TPropertyType>> property;

        internal CollectionPropertyMapping(
            ICollectionAggregation<TEntityType, TCollectionEntityType> collection,
            Expression<Func<TCollectionEntityType, TPropertyType>> property
        )
        {
            this.collection = collection;
            this.property = property;
        }

        public bool IsDefaultSortFilter { get; set; }

        public string Field => collection.GetField();

        public IQueryable<TEntityType> ApplySort(
            IQueryable<TEntityType> query,
            bool descending)
        {
            // TODO - how do we build the sort expression??
            return query;
        }

        public Expression<Func<TEntityType, bool>> GetFilterLambda(
            string value,
            DataOperator dataOperator)
        {
            var propertyPredicate = SearchHelper.GetLambdaExpression<TCollectionEntityType, TPropertyType>(
                property,
                value,
                dataOperator);

            return this.collection.GetPredicate<TPropertyType>(propertyPredicate);
        }
    }


    public interface ICollectionFilter<TRootEntityType, TCollectionEntityType>
        where TCollectionEntityType : class
    {
        Expression<Func<TRootEntityType, bool>> GetPredicate<TCollectionSubType>(
            Expression<Func<TCollectionEntityType, bool>> methodPredicate
        );

        string GetField();
    }
}