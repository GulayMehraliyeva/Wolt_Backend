using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Services;
using Service.Services.Interfaces;
using Service.ViewModels.Menu;
using Service.ViewModels.MenuCategory;
using Service.ViewModels.MenuItem;
using Service.ViewModels.Restaurant;
using Service.ViewModels.RestaurantReview;

namespace Wolt.Controllers
{
    [Authorize(Roles = "Customer, Admin")]
    public class MenuController : Controller
    {
        private readonly IRestaurantService _restaurantService;
        private readonly IMenuItemService _menuItemService;
        private readonly IMenuCategoryService _menuCategoryService;
        private readonly IRestaurantReviewService _restaurantReviewService;
        private readonly IAccountService _accountService;
        private readonly ICustomerService _customerService; 

        public MenuController(IRestaurantService restaurantService,
                              IMenuItemService menuItemService,
                              IMenuCategoryService menuCategoryService,
                              IRestaurantReviewService restaurantReviewService,
                              IAccountService accountService,
                              ICustomerService customerService)
        {
            _restaurantService = restaurantService;
            _menuItemService = menuItemService;
            _menuCategoryService = menuCategoryService;
            _restaurantReviewService = restaurantReviewService;
            _accountService = accountService;
            _customerService = customerService;
        }


        public async Task<IActionResult> Index(int restaurantId)
        {
            var menuItems = await _menuItemService.GetByRestaurantIdAsync(restaurantId);
            var menuCategories = await _menuCategoryService.GetByRestaurantIdAsync(restaurantId);
            var restaurant = await _restaurantService.GetByIdAsync(restaurantId);
            var reviews = await _restaurantReviewService.GetAllByRestaurantIdAsync(restaurantId);
            var currentUser = await _accountService.GetCurrentUserAsync();

            int? currentCustomerId = null;
            if (currentUser != null)
            {
                var customer = await _customerService.GetByUserIdAsync(currentUser.Id);
                currentCustomerId = customer?.Id;
            }

            ViewBag.CurrentCustomerId = currentCustomerId;

            var viewModel = new MenuVM
            {
                MenuItems = menuItems.Select(mi => new MenuItemVM
                {
                    Id = mi.Id,
                    Name = mi.Name,
                    Description = mi.Description,
                    Price = mi.Price,
                    CategoryName = mi.MenuItemCategory?.Name,
                    CategoryId = mi.CategoryId,
                    Image = mi.Image,
                    DiscountedPrice = mi.DiscountedPrice,

                }),

                MenuCategories = menuCategories.Select(c => new MenuCategoryVM
                {
                    Id = c.Id,
                    Name = c.Name
                }),

                Restaurant = new RestaurantVM
                {
                    Id = restaurant.Id,
                    Name = restaurant.Name,
                    Address = restaurant.Address,
                    Phone = restaurant.Phone,
                    Image = restaurant.Image,
                    EstimatedDeliveryTime = restaurant.EstimatedDeliveryTime,
                    DeliveryCost = restaurant.DeliveryCost,
                    CategoryName = restaurant.CategoryName,
                    RestaurantCategoryId = restaurant.RestaurantCategoryId,
                    AverageRating = restaurant.AverageRating
                },

                RestaurantReviews = reviews.Select(r => new RestaurantReviewVM
                {
                    RestaurantId = r.RestaurantId,
                    Comment = r.Comment,
                    Rating = r.Rating,
                    CustomerName = r.Customer?.User?.FullName,
                    Id = r.Id,
                    CustomerId = r.CustomerId,
                }),

                RestaurantReviewCreate = new RestaurantReviewCreateVM
                {
                    RestaurantId = restaurantId,
                }
            };

            return View(viewModel);
        }

    }
}
