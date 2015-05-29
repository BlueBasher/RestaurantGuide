using System.Configuration;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json.Serialization;
using RestaurantGuide.DataAccess;

namespace RestaurantGuide
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            GlobalConfiguration.Configure(WebApiConfig.Register);

            //Initialise Azure Search
            var restaurantIndexManager = new RestaurantIndexManager();
            restaurantIndexManager.InitialiseIndexForDocumentDb(
                ConfigurationManager.AppSettings["searchIndexName"],
                string.Format("AccountEndpoint={0};AccountKey={1};Database={2}",
                ConfigurationManager.AppSettings["endpoint"], ConfigurationManager.AppSettings["authKey"], ConfigurationManager.AppSettings["database"]),
                ConfigurationManager.AppSettings["collection"]).Wait();
        }
    }
}
