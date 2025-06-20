using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service.Services;
using Service.Services.Interfaces;
using Service.ViewModels.MenuCategory;
using Service.ViewModels.Restaurant;

namespace Wolt.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MenuCategoryController : Controller
    {
        private readonly IMenuCategoryService _menuCategoryService;
        private readonly IRestaurantService _restaurantService;
        private readonly ILogger<MenuCategoryController> _logger;

        public MenuCategoryController(IMenuCategoryService menuCategoryService,
                                      IRestaurantService restaurantService,
                                      ILogger<MenuCategoryController> logger)
        {
            _menuCategoryService = menuCategoryService;
            _restaurantService = restaurantService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? restaurantId)
        {
            _logger.LogInformation("MenuCategoryController: Index method used with restaurantId={RestaurantId}", restaurantId);

            var restaurants = await _restaurantService.GetAllAsync();

            var viewModel = new MenuCategoryIndexVM
            {
                Restaurants = restaurants.Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name
                }).ToList(),
                SelectedRestaurantId = restaurantId,
                MenuCategories = restaurantId.HasValue
                    ? await _menuCategoryService.GetByRestaurantIdAsync(restaurantId.Value)
                    : new List<MenuCategoryVM>()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("MenuCategoryController: Create GET method used");

            var restaurants = await _restaurantService.GetAllAsync();

            var viewModel = new MenuCategoryCreateVM
            {
                Restaurants = restaurants.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MenuCategoryCreateVM model)
        {
            _logger.LogInformation("MenuCategoryController: Create POST method used");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("MenuCategoryController: Create POST - Invalid model state");
                var restaurants = await _restaurantService.GetAllAsync();
                model.Restaurants = restaurants.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

                return View(model);
            }

            await _menuCategoryService.CreateAsync(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("MenuCategoryController: Edit GET method used for Id={Id}", id);

            var menuCategory = await _menuCategoryService.GetByIdAsync(id);
            if (menuCategory == null)
            {
                _logger.LogWarning("MenuCategoryController: Edit GET - MenuCategory not found for Id={Id}", id);
                return NotFound();
            }

            var restaurants = await _restaurantService.GetAllAsync();

            var editVm = new MenuCategoryEditVM
            {
                Id = menuCategory.Id,
                Name = menuCategory.Name,
                RestaurantId = menuCategory.RestaurantId,
                Restaurants = restaurants.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList()
            };

            return View(editVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MenuCategoryEditVM model)
        {
            _logger.LogInformation("MenuCategoryController: Edit POST method used for Id={Id}", id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("MenuCategoryController: Edit POST - Invalid model state for Id={Id}", id);
                var restaurants = await _restaurantService.GetAllAsync();
                model.Restaurants = restaurants.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

                return View(model);
            }

            await _menuCategoryService.EditAsync(id, model);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            _logger.LogInformation("MenuCategoryController: DeleteAjax method used for Id={Id}", id);

            await _menuCategoryService.DeleteAsync(id);
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            _logger.LogInformation("MenuCategoryController: Detail method used for Id={Id}", id);

            var menuCategory = await _menuCategoryService.GetByIdAsync(id);
            if (menuCategory == null)
            {
                _logger.LogWarning("MenuCategoryController: Detail - MenuCategory not found for Id={Id}", id);
                return NotFound();
            }

            return View(menuCategory);
        }
    }

}
