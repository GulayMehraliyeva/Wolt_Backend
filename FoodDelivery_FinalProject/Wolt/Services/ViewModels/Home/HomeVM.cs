using Service.ViewModels.Brand;
using Service.ViewModels.Restaurant;
using Service.ViewModels.Slider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.Home
{
    public class HomeVM
    {
        public IEnumerable<SliderVM> Sliders { get; set; }
        public IEnumerable<RestaurantVM> Restaurants { get; set; }
        public IEnumerable<BrandVM> Brands { get; set; }

    }
}
