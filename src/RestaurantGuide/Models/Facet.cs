using System.Collections.Generic;

namespace RestaurantGuide.Models
{
    public class FacetResults
    {
        public string Name { get; set; }

        public IEnumerable<Facet> Facets { get; set; }
    }

    public class Facet
    {
        public string Value { get; set; }

        public long Count { get; set; }
    }
}