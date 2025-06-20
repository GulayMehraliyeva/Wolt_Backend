using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;
using Service.ViewModels.RestaurantCategory;

namespace Wolt.Controllers
{
    [Authorize(Roles = "Customer, Admin")]
    public class RestaurantCategoryController : Controller
    {
        private readonly IRestaurantCategoryService _restaurantCategoryService;
        private readonly IRestaurantService _restaurantService;

        public RestaurantCategoryController(IRestaurantCategoryService restaurantCategoryService,
                                            IRestaurantService restaurantService)
        {
            _restaurantCategoryService = restaurantCategoryService;
            _restaurantService = restaurantService;
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var category = await _restaurantCategoryService.GetByIdAsync(id);
            if (category == null) return NotFound();

            var restaurants = await _restaurantService.GetByCategoryIdAsync(id);

            var vm = new RestaurantCategoryDetailVM
            {
                Category = category,
                Restaurants = restaurants.ToList()
            };

            return View(vm);
        }
    }

}
