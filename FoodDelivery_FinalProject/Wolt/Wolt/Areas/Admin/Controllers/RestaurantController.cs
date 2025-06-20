using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service.Services.Interfaces;
using Service.ViewModels.Restaurant;

namespace Wolt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RestaurantController : Controller
    {
        private readonly IRestaurantService _restaurantService;
        private readonly IRestaurantCategoryService _restaurantCategoryService;
        public RestaurantController(IRestaurantService restaurantService,
                                    IRestaurantCategoryService restaurantCategoryService)
        {
            _restaurantService = restaurantService;
            _restaurantCategoryService = restaurantCategoryService;
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var restaurants = await _restaurantService.GetAllAsync();
            return View(restaurants);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var categories = await _restaurantCategoryService.GetAllAsync(); 

            var viewModel = new RestaurantCreateVM
            {
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(RestaurantCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _restaurantCategoryService.GetAllAsync();
                model.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

                return View(model);
            }

            await _restaurantService.CreateAsync(model);
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var restaurant = await _restaurantService.GetByIdAsync(id);
            if (restaurant == null) return NotFound();

            var categories = await _restaurantCategoryService.GetAllAsync();

            var editVm = new RestaurantEditVM
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Address = restaurant.Address,
                Phone = restaurant.Phone,
                EstimatedDeliveryTime = restaurant.EstimatedDeliveryTime,
                DeliveryCost = restaurant.DeliveryCost,
                ExistingImage = restaurant.Image,
                RestaurantCategoryId = restaurant.RestaurantCategoryId,
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList()
            };

            return View(editVm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, RestaurantEditVM model)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _restaurantCategoryService.GetAllAsync();
                model.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

                return View(model);
            }

            await _restaurantService.EditAsync(id, model);
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _restaurantService.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var restaurant = await _restaurantService.GetByIdAsync(id);
            if (restaurant == null) return NotFound();

            return View(restaurant);
        }
    }
}

