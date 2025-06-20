using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.MenuCategory
{
    public class MenuCategoryIndexVM
    {
        public List<SelectListItem> Restaurants { get; set; }
        public int? SelectedRestaurantId { get; set; }
        public IEnumerable<MenuCategoryVM> MenuCategories { get; set; }
    }
}
