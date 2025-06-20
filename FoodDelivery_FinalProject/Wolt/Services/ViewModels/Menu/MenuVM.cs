using Service.ViewModels.MenuCategory;
using Service.ViewModels.MenuItem;
using Service.ViewModels.Restaurant;
using Service.ViewModels.RestaurantReview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.Menu
{
    public class MenuVM
    {
        public IEnumerable<MenuCategoryVM> MenuCategories { get; set; }
        public IEnumerable<MenuItemVM> MenuItems { get; set; }
        public RestaurantVM Restaurant { get; set; }
        public RestaurantReviewCreateVM RestaurantReviewCreate { get; set; }
        public IEnumerable<RestaurantReviewVM> RestaurantReviews { get; set; }

    }
}
