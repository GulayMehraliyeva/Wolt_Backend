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
        private readonly IRestaurantService _restaurantService;
        private readonly ILogger<RestaurantReviewController> _logger;

        public RestaurantReviewController(IRestaurantReviewService restaurantReviewService,
                                          IRestaurantService restaurantService,
                                          ILogger<RestaurantReviewController> logger)
        {
            _restaurantReviewService = restaurantReviewService;
            _restaurantService = restaurantService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("RestaurantReviewController: Index method used");
            var restaurants = await _restaurantService.GetAllAsync();
            return View(restaurants);
        }

        [HttpGet]
        public async Task<IActionResult> ReviewsByRestaurant(int restaurantId)
        {
            _logger.LogInformation("RestaurantReviewController: ReviewsByRestaurant method used for RestaurantId={RestaurantId}", restaurantId);
            var reviews = await _restaurantReviewService.GetAllByRestaurantIdAsync(restaurantId);
            ViewBag.RestaurantId = restaurantId;
            return View(reviews);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int restaurantId)
        {
            _logger.LogInformation("RestaurantReviewController: Delete method used for ReviewId={ReviewId}, RestaurantId={RestaurantId}", id, restaurantId);
            await _restaurantReviewService.DeleteAsync(id);
            return RedirectToAction(nameof(ReviewsByRestaurant), new { restaurantId });
        }
    }

}
