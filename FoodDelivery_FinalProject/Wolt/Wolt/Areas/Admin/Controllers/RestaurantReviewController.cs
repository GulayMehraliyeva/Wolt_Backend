using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service.Services.Interfaces;
using Service.ViewModels.RestaurantReview;

namespace Wolt.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RestaurantReviewController : Controller
    {
        private readonly IRestaurantReviewService _restaurantReviewService;
        private readonly IRestaurantService _restaurantService; // to get restaurants list

        public RestaurantReviewController(IRestaurantReviewService restaurantReviewService,
                                          IRestaurantService restaurantService)
        {
            _restaurantReviewService = restaurantReviewService;
            _restaurantService = restaurantService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var restaurants = await _restaurantService.GetAllAsync();
            return View(restaurants);
        }

        [HttpGet]
        public async Task<IActionResult> ReviewsByRestaurant(int restaurantId)
        {
            var reviews = await _restaurantReviewService.GetAllByRestaurantIdAsync(restaurantId);
            ViewBag.RestaurantId = restaurantId;
            return View(reviews);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int restaurantId)
        {
            await _restaurantReviewService.DeleteAsync(id);
            return RedirectToAction(nameof(ReviewsByRestaurant), new { restaurantId });
        }
    }
}
