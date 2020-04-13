namespace Exelor.Infrastructure.Data.DataProcessor.Mapping
{
    public class CollectionMapping<TEntityType>
    {
        private readonly string field;

        public CollectionMapping(
            string field)
        {
            this.field = field;
        }

        internal string GetField()
        {
            return field;
        }
    }
}