using RestaurantGuide.DataAccess;
using RestaurantGuide.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace RestaurantGuide.Controllers.Api
{
    public class SearchController : ApiController
    {
        private readonly RestaurantSearchRepository _restaurantSearchRepository;
        public SearchController()
        {
            _restaurantSearchRepository = new RestaurantSearchRepository();
        }

        public async Task<SearchResults<Restaurant>> Get(string searchTerm, [FromUri]string[] cuisines)
        {
            return await Get(searchTerm, null, cuisines);
        }

        public async Task<SearchResults<Restaurant>> Get(string searchTerm, [FromUri]int? michelinStars, [FromUri]string[] cuisines)
        {
            var searchResponse = await _restaurantSearchRepository.SearchAsync(searchTerm, michelinStars, cuisines);

            var searchResults = new SearchResults<Restaurant>
            {
                Results = searchResponse.Results,
                Facets = searchResponse.Facets.Select(f => new FacetResults{ Name=f.Key,  Facets = f.Value.Select( v => new Facet{ Value = v.Value.ToString(), Count = v.Count})})
            };

            return searchResults;
        }
    }
}
