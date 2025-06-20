using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service.Helpers.Exceptions;
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
        private readonly ILogger<RestaurantController> _logger;

        public RestaurantController(IRestaurantService restaurantService,
                                    IRestaurantCategoryService restaurantCategoryService,
                                    ILogger<RestaurantController> logger)
        {
            _restaurantService = restaurantService;
            _restaurantCategoryService = restaurantCategoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("RestaurantController: Index method used");
            var restaurants = await _restaurantService.GetAllAsync();
            return View(restaurants);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("RestaurantController: Create GET method used");
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
        public async Task<IActionResult> Create(RestaurantCreateVM model)
        {
            _logger.LogInformation("RestaurantController: Create POST method used");

            var categories = await _restaurantCategoryService.GetAllAsync();
            model.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _restaurantService.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (AppValidationException ex)
            {
                ModelState.AddModelError("Name", ex.Message);
                return View(model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("RestaurantController: Edit GET method used");
            var restaurant = await _restaurantService.GetByIdAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }

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
        public async Task<IActionResult> Edit(int id, RestaurantEditVM model)
        {
            _logger.LogInformation("RestaurantController: Edit POST method used");

            var categories = await _restaurantCategoryService.GetAllAsync();
            model.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _restaurantService.EditAsync(id, model);
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
            _logger.LogInformation("RestaurantController: Delete method used for Id={Id}", id);
            await _restaurantService.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            _logger.LogInformation("RestaurantController: Detail method used");
            var restaurant = await _restaurantService.GetByIdAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            return View(restaurant);
        }
    }

}

