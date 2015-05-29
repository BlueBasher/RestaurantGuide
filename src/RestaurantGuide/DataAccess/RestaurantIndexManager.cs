using System.Configuration;
using RestaurantGuide.Models;

namespace RestaurantGuide.DataAccess
{
    public class RestaurantIndexManager : SearchIndexManagerBase<Restaurant>
    {
        public RestaurantIndexManager()
            : base(ConfigurationManager.AppSettings["searchUrl"], ConfigurationManager.AppSettings["searchApiKey"])
        {
        }

    }
}