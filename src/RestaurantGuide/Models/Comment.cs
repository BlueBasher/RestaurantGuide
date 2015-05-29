using System.Collections.Generic;
using Newtonsoft.Json;

namespace RestaurantGuide.Models
{
    public class Comment
    {
        [JsonProperty(PropertyName = "userName")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "comment")]
        public string Description { get; set; }
        
        [JsonProperty(PropertyName = "comments")]
        public IEnumerable<Comment> Comments { get; set; }
    }
}