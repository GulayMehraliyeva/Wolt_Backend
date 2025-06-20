using Service.ViewModels.Restaurant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.RestaurantCategory
{
    public class RestaurantCategoryDetailVM
    {
        public RestaurantCategoryVM Category { get; set; }
        public List<RestaurantVM> Restaurants { get; set; }
    }
}
