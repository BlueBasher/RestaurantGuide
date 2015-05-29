using System.Collections.Generic;
using Microsoft.Azure.Search.Models;

namespace RestaurantGuide.Models
{
    public class SearchResults<T> where T : class
    {
        public IEnumerable<SearchResult<T>> Results { get; set; }

        public IEnumerable<FacetResults> Facets { get; set; }
    }
}