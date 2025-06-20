using Service.ViewModels.RestaurantCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.Restaurant
{
    public class RestaurantVMUI
    {
        public IEnumerable<RestaurantCategoryVM> RestaurantCategories { get; set; }
        public IEnumerable<RestaurantVM> Restaurants { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
