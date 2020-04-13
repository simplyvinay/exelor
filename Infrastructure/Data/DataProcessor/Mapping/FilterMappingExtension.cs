using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Exelor.Infrastructure.Data.DataProcessor.Mapping
{
    public static class FilterMappingExtensions
    {
        public static ICollectionAggregation<TRootEntityType, TCollectionEntityType>
            Any<TRootEntityType, TCollectionEntityType>(
                this CollectionMapping<TRootEntityType> collectionRootMapping,
                Expression<Func<TRootEntityType, ICollection<TCollectionEntityType>>> collectionExpression
            )
            where TRootEntityType : class
            where TCollectionEntityType : class
        {
            return new CollectionAggregation<TRootEntityType, TRootEntityType, TCollectionEntityType>(
                AggregateOperation.Any,
                collectionRootMapping,
                collectionExpression
            );
        }

        public static ICollectionAggregation<TRootEntityType, TCollectionEntityCollectionType>
            Any<TRootEntityType, TCollectionEntityType, TCollectionEntityCollectionType>(
                this ICollectionAggregation<TRootEntityType, TCollectionEntityType> collection,
                Expression<Func<TCollectionEntityType, ICollection<TCollectionEntityCollectionType>>>
                    collectionExpression
            )
            where TRootEntityType : class
            where TCollectionEntityType : class
            where TCollectionEntityCollectionType : class
        {
            return new CollectionAggregation<TRootEntityType, TCollectionEntityType, TCollectionEntityCollectionType>(
                AggregateOperation.Any,
                collection,
                collectionExpression
            );
        }

        public static CollectionPropertyMapping<TEntityType, TCollectionEntityType, TPropType> Property<TEntityType,
            TCollectionEntityType, TPropType>(
            this ICollectionAggregation<TEntityType, TCollectionEntityType> collection,
            Expression<Func<TCollectionEntityType, TPropType>> cpop
        )
            where TEntityType : class
            where TCollectionEntityType : class
        {
            // todo, check cpop is actually a property

            return new CollectionPropertyMapping<TEntityType, TCollectionEntityType, TPropType>(
                collection,
                cpop);
        }
    }
}