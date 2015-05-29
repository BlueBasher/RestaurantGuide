using System.Collections.Generic;
using Microsoft.Runtime.CompilerServices;
using Newtonsoft.Json;
using RestaurantGuide.Search;

namespace RestaurantGuide.Models
{
    public class Restaurant
    {
        // Setting custom Json Serializer Settings for DocumentDb is not available
        // http://feedback.azure.com/forums/263030-documentdb/suggestions/6422364-allow-me-to-set-jsonserializersettings
        // Therfor we MUST set at least Id to id. Objects with no "id"-property can not be updated using ReplaceDocumentAsync.
        [JsonProperty(PropertyName = "id")]
        [SearchIndex(IsKey = true, IsRetrievable = true)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        [SearchIndex(IsRetrievable = true, IsSearchable = true)]
        [SearchSuggestion(SuggesterName = "restaurantsuggester")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "address")]
        [SearchIndex(IsRetrievable = true, IsSearchable = true)]
        public string Address { get; set; }

        [JsonProperty(PropertyName = "city")]
        [SearchIndex(IsRetrievable = true, IsSearchable = true)]
        [SearchSuggestion(SuggesterName = "restaurantsuggester")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "imageUrl")]
        [SearchIndex(IsRetrievable = true)]
        public string ImageUrl { get; set; }

        [JsonProperty(PropertyName = "michelinStars")]
        [SearchIndex(IsRetrievable = true, IsFacetable = true, IsFilterable = true)]
        public int MichelinStars { get; set; }

        [JsonProperty(PropertyName = "geoLocation")]
        public GeoLocation GeoLocation { get; set; }

        [JsonProperty(PropertyName = "cuisines")]
        [SearchIndex(IsRetrievable = true, IsFacetable = true, IsFilterable = true)]
        public CuisineCollection Cuisines { get; set; }

        [JsonProperty(PropertyName = "ratings")]
        public IEnumerable<Rating> Ratings { get; set; }

        [JsonProperty(PropertyName = "referenceUrl")]
        [SearchIndex(IsRetrievable = true)]
        public string ReferenceUrl { get; set; }
    }
}