using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.MenuCategory
{
    public class MenuCategoryVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RestaurantName { get; set; }
        public int RestaurantId { get; set; }
    }
}
