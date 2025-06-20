using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.RestaurantCategory
{
    public class RestaurantCategoryVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int NumberOfRestaurants { get; set; }
    }
}
