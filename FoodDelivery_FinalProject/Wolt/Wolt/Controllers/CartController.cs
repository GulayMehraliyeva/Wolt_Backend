using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Services;
using Service.Services.Interfaces;
using Service.ViewModels.Cart;
using System.Security.Claims;

namespace Wolt.Controllers
{
    [Authorize(Roles = "Customer, Admin")]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ICustomerService _customerService;
        private readonly IMenuItemService _menuItemService;

        public CartController(ICartService cartService, ICustomerService customerService, IMenuItemService menuItemService)
        {
            _cartService = cartService;
            _customerService = customerService;
            _menuItemService = menuItemService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private async Task<int> GetLoggedInCustomerIdAsync()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("User not logged in.");

            var customer = await _customerService.GetByUserIdAsync(userId);
            if (customer == null)
                throw new Exception("Customer not found.");

            return customer.Id;
        }

        public async Task<IActionResult> Index()
        {
            int customerId = await GetLoggedInCustomerIdAsync();
            var cart = await _cartService.GetCartAsync(customerId);
            return View(cart);
        }

        public async Task<IActionResult> Add(int menuItemId)
        {
            int customerId = await GetLoggedInCustomerIdAsync();

            var menuItem = await _menuItemService.GetByIdAsync(menuItemId);
            if (menuItem == null) return NotFound();

            int restaurantId = menuItem.RestaurantId;

            try
            {
                await _cartService.AddToCartAsync(customerId, menuItemId);
            }
            catch (Exception ex)
            {
                if (ex.Message == "You can only order from one restaurant.")
                {
                    TempData["Error"] = "Səbətinizdə başqa restoranın məhsulu var. Zəhmət olmasa əvvəlcə səbəti təmizləyin.";
                    return RedirectToAction("Index");
                }

                throw;
            }

            return RedirectToAction("Index", "Menu", new { restaurantId });
        }


        public async Task<IActionResult> Clear()
        {
            int customerId = await GetLoggedInCustomerIdAsync();
            await _cartService.ClearCartAsync(customerId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateCartItemDto dto)
        {
            await _cartService.UpdateCartItemQuantityAsync(dto.CartItemId, dto.Quantity);
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            var cartItem = await _cartService.GetCartItemsAsync(itemId);
            if (cartItem != null)
            {
                await _cartService.RemoveCartItemAsync(itemId);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckAndRedirectToOrder()
        {
            var customerId = await GetLoggedInCustomerIdAsync();
            var cart = await _cartService.GetCartAsync(customerId);

            if (cart == null || cart.Items == null || !cart.Items.Any())
            {
                TempData["Error"] = "Səbət boşdur.";
                return RedirectToAction("Index");
            }

            return RedirectToAction("PlaceOrder", "Order");
        }

    }
}
