using Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service.Helpers.Exceptions;
using Service.Services;
using Service.Services.Interfaces;
using Service.ViewModels.MenuItem;
using Service.ViewModels.Restaurant;

namespace Wolt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MenuItemController : Controller
    {
        private readonly IMenuItemService _menuItemService;
        private readonly IMenuCategoryService _menuCategoryService;
        private readonly IDiscountService _discountService;
        private readonly IRestaurantService _restaurantService;
        private readonly ILogger<MenuItemController> _logger;

        public MenuItemController(IMenuItemService menuItemService,
                                  IMenuCategoryService menuCategoryService,
                                  IDiscountService discountService,
                                  IRestaurantService restaurantService,
                                  ILogger<MenuItemController> logger)
        {
            _menuItemService = menuItemService;
            _menuCategoryService = menuCategoryService;
            _discountService = discountService;
            _restaurantService = restaurantService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoriesByRestaurant(int restaurantId)
        {
            _logger.LogInformation("MenuItemController: GetCategoriesByRestaurant used for RestaurantId={RestaurantId}", restaurantId);

            var allCategories = await _menuCategoryService.GetAllAsync();
            var categories = allCategories.Where(mc => mc.RestaurantId == restaurantId).ToList();

            var result = categories.Select(c => new
            {
                id = c.Id,
                name = c.Name
            });

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetMenuItemsByCategory(int categoryId)
        {
            _logger.LogInformation("MenuItemController: GetMenuItemsByCategory used for CategoryId={CategoryId}", categoryId);

            var menuItems = await _menuItemService.GetByCategoryIdAsync(categoryId);
            var result = menuItems.Select(m => new
            {
                m.Id,
                m.Name,
                m.Description,
                m.Price,
                m.Image,
                m.CategoryName
            });
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? restaurantId)
        {
            _logger.LogInformation("MenuItemController: Index method used");

            var menuItems = await _menuItemService.GetAllAsync();
            var restaurants = await _restaurantService.GetAllAsync();

            ViewBag.Restaurants = new SelectList(restaurants, "Id", "Name");

            return View(menuItems);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("MenuItemController: Create GET method used");

            var categories = await _menuCategoryService.GetAllAsync();
            var discounts = await _discountService.GetAllAsync();
            var restaurants = await _restaurantService.GetAllAsync();

            var viewModel = new MenuItemCreateVM
            {
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList(),

                Discounts = discounts.Where(d => d.IsActive).Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList(),

                Restaurants = restaurants.Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MenuItemCreateVM model)
        {
            _logger.LogInformation("MenuItemController: Create POST method used");

            var categories = await _menuCategoryService.GetAllAsync();
            var discounts = await _discountService.GetAllAsync();
            var restaurants = await _restaurantService.GetAllAsync();

            model.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            model.Discounts = discounts.Where(d => d.IsActive).Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name
            }).ToList();

            model.Restaurants = restaurants.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.Name
            }).ToList();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _menuItemService.CreateAsync(model);
                return RedirectToAction("Index");
            }
            catch (AppValidationException ex)
            {
                if (!ModelState.ContainsKey("Name") ||
                    !ModelState["Name"].Errors.Any(e => e.ErrorMessage == ex.Message))
                {
                    ModelState.AddModelError("Name", ex.Message);
                }

                return View(model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("MenuItemController: Edit GET method used");

            var menuItem = await _menuItemService.GetByIdAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            var allCategories = await _menuCategoryService.GetAllAsync();
            var allDiscounts = await _discountService.GetAllAsync();
            var allRestaurants = await _restaurantService.GetAllAsync();

            var model = new MenuItemEditVM
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                CategoryId = menuItem.CategoryId,
                RestaurantId = menuItem.RestaurantId,
                DiscountIds = menuItem.DiscountIds,
                ExistingImage = menuItem.Image,

                Categories = allCategories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList(),

                Discounts = allDiscounts.Where(d => d.IsActive).Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                }).ToList(),

                Restaurants = allRestaurants.Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MenuItemEditVM model)
        {
            _logger.LogInformation("MenuItemController: Edit POST method used");

            if (model.DiscountIds == null)
                model.DiscountIds = new List<int>();

            var categories = await _menuCategoryService.GetAllAsync();
            var discounts = await _discountService.GetAllAsync();
            var restaurants = await _restaurantService.GetAllAsync();

            model.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            model.Discounts = discounts.Where(d => d.IsActive).Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name
            }).ToList();

            model.Restaurants = restaurants.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.Name
            }).ToList();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _menuItemService.EditAsync(model);
                return RedirectToAction("Index");
            }
            catch (AppValidationException ex)
            {
                if (!ModelState.ContainsKey("Name") ||
                    !ModelState["Name"].Errors.Any(e => e.ErrorMessage == ex.Message))
                {
                    ModelState.AddModelError("Name", ex.Message);
                }

                return View(model);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("MenuItemController: Delete method used");

            await _menuItemService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            _logger.LogInformation("MenuItemController: Detail method used for Id={Id}", id);

            try
            {
                var menuItemDetail = await _menuItemService.DetailAsync(id);
                return View(menuItemDetail);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }

}
