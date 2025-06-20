using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.Restaurant
{
    public class RestaurantFilterVM
    {
        public bool? FreeDelivery { get; set; }
        public int? MaxDeliveryTime { get; set; }
        public string SortBy { get; set; }
        public List<int> SelectedRatings { get; set; }
    }
}
