using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using RestaurantGuide.DataAccess;

namespace RestaurantGuide.Controllers.Api
{
    public class SuggestController : ApiController
    {
        private readonly RestaurantSearchRepository _restaurantSearchRepository;

        public SuggestController()
        {
            _restaurantSearchRepository = new RestaurantSearchRepository();
        }


        public async Task<IEnumerable<string>> Get(string searchText)
        {
            return await _restaurantSearchRepository.GetSuggestionsAsync(searchText);
        }
    }
}
