using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Helpers.Exceptions;
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
                return View(request);

            try
            {
                await _restaurantCategoryService.CreateAsync(request);
                return RedirectToAction(nameof(Index));
            }
            catch (AppValidationException ex)
            {
                ModelState.AddModelError("Name", ex.Message);
                return View(request);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("RestaurantCategoryController: Delete method used");
            await _restaurantCategoryService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("RestaurantCategoryController: Edit GET method used");

            var restaurantCategory = await _restaurantCategoryService.GetByIdAsync(id);
            if (restaurantCategory == null)
            {
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
            _logger.LogInformation("RestaurantCategoryController: Edit POST method used");

            if (!ModelState.IsValid)
                return View(editVm);

            try
            {
                await _restaurantCategoryService.EditAsync(editVm.Id, editVm);
                return RedirectToAction(nameof(Index));
            }
            catch (AppValidationException ex)
            {
                if (!ModelState.ContainsKey("Name") ||
                    !ModelState["Name"].Errors.Any(e => e.ErrorMessage == ex.Message))
                {
                    ModelState.AddModelError("Name", ex.Message);
                }
                return View(editVm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            _logger.LogInformation("RestaurantCategoryController: Detail method used");

            var category = await _restaurantCategoryService.GetByIdAsync(id);
            return View(category);
        }
    }

}
