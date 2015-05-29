using RestaurantGuide.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace RestaurantGuide.DataAccess
{
    public class RestaurantRepository : DocumentDBRepositoryBase<Restaurant>
    {
        public RestaurantRepository()
        {
            DatabaseId = ConfigurationManager.AppSettings["database"];
            CollectionId = ConfigurationManager.AppSettings["collection"];
        }
    }
}