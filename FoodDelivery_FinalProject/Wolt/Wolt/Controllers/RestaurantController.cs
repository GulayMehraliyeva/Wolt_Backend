using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Services;
using Service.Services.Interfaces;
using Service.ViewModels.Home;
using Service.ViewModels.Restaurant;

namespace Wolt.Controllers
{
    [Authorize(Roles = "Customer, Admin")]
    public class RestaurantController : Controller
    {
        private readonly IRestaurantService _restaurantService;
        private readonly IRestaurantCategoryService _restaurantCategoryService;

        public RestaurantController(IRestaurantService restaurantService,
                                    IRestaurantCategoryService restaurantCategoryService)
        {
            _restaurantService = restaurantService;
            _restaurantCategoryService = restaurantCategoryService;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            var vm = await _restaurantService.GetAllPaginatedWithCategories(page);
            return View(vm);
        }

        public async Task<IActionResult> GetRestaurantsPage(int page = 1)
        {
            var vm = await _restaurantService.GetAllPaginatedWithCategories(page);
            return PartialView("_RestaurantsListPartial", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Filter(RestaurantFilterVM filter)
        {
            var filteredRestaurants = await _restaurantService.GetFilteredAsync(filter);
            return PartialView("_FilteredRestaurantsPartial", filteredRestaurants);
        }

        [HttpGet]
        public async Task<IActionResult> GetSortedRestaurants(string sortOrder = "newest", int page = 1)
        {
            var allRestaurants = await _restaurantService.GetAllPaginatedWithCategories(page);
            var sorted = SortRestaurants(allRestaurants.Restaurants, sortOrder);

            allRestaurants.Restaurants = sorted.ToList();

            return PartialView("_RestaurantsListPartial", allRestaurants);
        }

        private IEnumerable<RestaurantVM> SortRestaurants(IEnumerable<RestaurantVM> list, string sortOrder)
        {
            return sortOrder switch
            {
                "newest" => list.OrderByDescending(r => r.Id),
                "oldest" => list.OrderBy(r => r.Id),
                "name_asc" => list.OrderBy(r => r.Name),
                "name_desc" => list.OrderByDescending(r => r.Name),
                "rating_high" => list.OrderByDescending(r => r.AverageRating),
                "rating_low" => list.OrderBy(r => r.AverageRating),
                _ => list.OrderByDescending(r => r.Id)
            };
        }

    }
}
