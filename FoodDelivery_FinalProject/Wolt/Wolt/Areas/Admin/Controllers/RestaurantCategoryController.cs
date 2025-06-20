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
        private readonly ILogger<RestaurantCategoryController> _logger;

        public RestaurantCategoryController(IRestaurantCategoryService restaurantCategoryService,
                                            ILogger<RestaurantCategoryController> logger)
        {
            _restaurantCategoryService = restaurantCategoryService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("RestaurantCategoryController: Index method used");
            var categories = await _restaurantCategoryService.GetAllAsync();
            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("RestaurantCategoryController: Create GET method used");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RestaurantCategoryCreateVM request)
        {
            _logger.LogInformation("RestaurantCategoryController: Create POST method used");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("RestaurantCategoryController: Create POST - Invalid model state");
                return View(request);
            }

            await _restaurantCategoryService.CreateAsync(request);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("RestaurantCategoryController: Delete method used for Id={Id}", id);
            await _restaurantCategoryService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("RestaurantCategoryController: Edit GET method used for Id={Id}", id);

            var restaurantCategory = await _restaurantCategoryService.GetByIdAsync(id);
            if (restaurantCategory == null)
            {
                _logger.LogWarning("RestaurantCategoryController: Edit GET - Category not found for Id={Id}", id);
                return NotFound();
            }

            var editVm = new RestaurantCategoryEditVM
            {
                Id = restaurantCategory.Id,
                Name = restaurantCategory.Name,
                CurrentImagePath = restaurantCategory.Image,
            };

            return View(editVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RestaurantCategoryEditVM editVm)
        {
            _logger.LogInformation("RestaurantCategoryController: Edit POST method used for Id={Id}", editVm.Id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("RestaurantCategoryController: Edit POST - Invalid model state for Id={Id}", editVm.Id);
                return View(editVm);
            }

            try
            {
                await _restaurantCategoryService.EditAsync(editVm.Id, editVm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RestaurantCategoryController: Edit POST - Exception for Id={Id}", editVm.Id);
                ModelState.AddModelError("", ex.Message);
                return View(editVm);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            _logger.LogInformation("RestaurantCategoryController: Detail method used for Id={Id}", id);

            var category = await _restaurantCategoryService.GetByIdAsync(id);
            return View(category);
        }
    }

}
