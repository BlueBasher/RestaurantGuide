using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Search.Models;
using RestaurantGuide.Models;

namespace RestaurantGuide.DataAccess
{
    public class RestaurantSearchRepository : SearchRepositoryBase<Restaurant>
    {
        public RestaurantSearchRepository()
            : base(ConfigurationManager.AppSettings["searchIndexName"])
        {
        }

        public async Task<DocumentSearchResponse<Restaurant>> SearchAsync(string searchTerm, int? michelinStars, string[] cuisines)
        {
            string filter = null;

            if (michelinStars.HasValue)
            {
                filter = String.Format("MichelinStars eq {0}", michelinStars.Value);
            }

            if (cuisines != null && cuisines.Any())
            {
                var cuisinesFilter = string.Join(" and ",
                    cuisines.Select(val => string.Format("Cuisines/any(c: c eq '{0}')", val)));

                if (filter == null)
                {
                    filter = cuisinesFilter;
                }
                else
                {
                    filter = string.Concat(cuisinesFilter, " and ", filter);
                }
            }

            return await SearchAsync(searchTerm, filter);
        }

        public async Task<IEnumerable<string>> GetSuggestionsAsync(string searchText)
        {
            return await GetSuggestionsAsync("restaurantsuggester", searchText);
        }

    }
}