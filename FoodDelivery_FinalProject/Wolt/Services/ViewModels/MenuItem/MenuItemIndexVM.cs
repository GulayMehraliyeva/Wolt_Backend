using Service.ViewModels.MenuCategory;
using Service.ViewModels.Restaurant;
using Service.ViewModels.RestaurantCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.MenuItem
{
    public class MenuItemIndexVM
    {
        public IEnumerable<RestaurantVM> Restaurants { get; set; }
        public IEnumerable<MenuCategoryVM> Categories { get; set; }
        public IEnumerable<MenuItemVM> MenuItems { get; set; }
    }
}
