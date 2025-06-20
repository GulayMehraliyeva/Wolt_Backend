using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;
using Service.ViewModels.RestaurantReview;

namespace Wolt.Controllers
{
    [Authorize(Roles = "Customer, Admin")]
    public class RestaurantReviewController : Controller
    {
        private readonly IRestaurantReviewService _reviewService;

        public RestaurantReviewController(IRestaurantReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create(RestaurantReviewCreateVM model)
        {
            try
            {
                await _reviewService.CreateAsync(model);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ReviewError"] = ex.Message;
            }

            return RedirectToAction("Index", "Menu", new { restaurantId = model.RestaurantId });
        }


        [HttpPost]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _reviewService.GetByIdAsync(id);
            if (review == null)
                return NotFound();

            await _reviewService.DeleteAsync(id);
            return RedirectToAction("Index", "Menu", new { restaurantId = review.RestaurantId });
        }
    }
}
