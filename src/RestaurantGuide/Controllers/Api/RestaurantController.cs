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
    public class RestaurantController : ApiController
    {
        private readonly RestaurantRepository _restaurantRepository;
        public RestaurantController()
        {
            _restaurantRepository = new RestaurantRepository();
        }

        // GET: api/Restaurant
        public IEnumerable<Restaurant> Get()
        {
            return _restaurantRepository.GetItems();
        }

        // GET: api/Restaurant/5
        public Restaurant Get(string id)
        {
            var item = _restaurantRepository.GetItem(r => r.Id == id);
            return item;
        }

        // POST: api/Restaurant
        public async Task<IHttpActionResult> Post([FromBody]Restaurant restaurant)
        {
            var createdId = await _restaurantRepository.CreateItemAsync(restaurant);
            restaurant.Id = createdId;
            return Ok(restaurant);
        }

        // PUT api/Restaurant/5
        public async Task<IHttpActionResult> Put(string id, [FromBody]Restaurant restaurant)
        {
            await _restaurantRepository.UpdateItemAsync(id, restaurant);
            return Ok();
        }

        // DELETE api/Restaurant/5
        public async Task<IHttpActionResult> Delete(string id)
        {
            await _restaurantRepository.DeleteItemAsync(id);
            return Ok();
        }
    }
}
