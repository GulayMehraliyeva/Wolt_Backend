using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Services;
using Service.Services.Interfaces;
using Service.ViewModels.RestaurantCategory;
using Service.ViewModels.Slider;

namespace Wolt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RestaurantCategoryController : Controller
    { 
        private readonly IRestaurantCategoryService _restaurantCategoryService;
        public RestaurantCategoryController(IRestaurantCategoryService restaurantCategoryService)
        {
            _restaurantCategoryService = restaurantCategoryService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _restaurantCategoryService.GetAllAsync();
            return View(categories);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RestaurantCategoryCreateVM request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            await _restaurantCategoryService.CreateAsync(request);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _restaurantCategoryService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var restaurantCategory = await _restaurantCategoryService.GetByIdAsync(id);
            if (restaurantCategory == null) return NotFound();

            var editVm = new RestaurantCategoryEditVM
            {
                Id = restaurantCategory.Id,
                Name = restaurantCategory.Name,
                CurrentImagePath = restaurantCategory.Image,
            };

            return View(editVm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RestaurantCategoryEditVM editVm)
        {
            if (!ModelState.IsValid)
            {
                return View(editVm);
            }

            try
            {
                await _restaurantCategoryService.EditAsync(editVm.Id, editVm);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(editVm);
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var category = await _restaurantCategoryService.GetByIdAsync(id);
            return View(category);
        }
    }
}
