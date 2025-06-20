using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;
using Service.ViewModels.Home;
using Service.ViewModels.Slider;

namespace Wolt.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISliderService _sliderService;
        private readonly IRestaurantService _restaurantService;
        public HomeController(ISliderService sliderService, IRestaurantService restaurantService)
        {
            _sliderService = sliderService;
            _restaurantService = restaurantService;
        }
        public async Task<IActionResult> Index()
        {
            var sliders = await _sliderService.GetAllAsync();
            var restaurants = await _restaurantService.GetAllAsync();

            var homeVM = new HomeVM
            {
                Sliders = sliders,
                Restaurants = restaurants
            };

            return View(homeVM);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            var results = await _restaurantService.SearchRestaurantsAsync(query);
            return View("Search", results);
        }
    }
}
