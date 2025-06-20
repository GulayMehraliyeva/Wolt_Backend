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
        public MenuCategoryController(IMenuCategoryService menuCategoryService,
                                      IRestaurantService restaurantService)
        {
            _menuCategoryService = menuCategoryService;
            _restaurantService = restaurantService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Index(int? restaurantId)
        {
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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MenuCategoryCreateVM model)
        {
            if (!ModelState.IsValid)
            {
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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var menuCategory = await _menuCategoryService.GetByIdAsync(id);
            if (menuCategory == null) return NotFound();

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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MenuCategoryEditVM model)
        {
            if (!ModelState.IsValid)
            {
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            await _menuCategoryService.DeleteAsync(id);
            return Json(new { success = true });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var menuCategory = await _menuCategoryService.GetByIdAsync(id);
            if (menuCategory == null) return NotFound();

            return View(menuCategory);
        }
    }
}
