using System.Collections.Generic;

namespace Exelor.Infrastructure.Data.DataProcessor
{
    public class SearchResult<T>
    {
        public List<T> Results { get; set; }
        public int TotalResults { get; set; }
    }
}