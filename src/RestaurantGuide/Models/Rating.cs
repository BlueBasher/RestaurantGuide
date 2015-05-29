using Newtonsoft.Json;

namespace RestaurantGuide.Models
{
    public class Rating : Comment
    {
        [JsonProperty(PropertyName = "numberOfStars")]
        public int NumberOfStars { get; set; }
    }
}