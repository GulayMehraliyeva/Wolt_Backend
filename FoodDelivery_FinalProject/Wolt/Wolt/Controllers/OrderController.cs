using Domain.Helpers.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repository.Repositories;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;
using Service.ViewModels.Order;
using System.Security.Claims;

namespace Wolt.Controllers
{
    [Authorize(Roles = "Customer, Admin")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly ICustomerService _customerService;

        public OrderController(
            IOrderService orderService,
            ICartService cartService,
            ICustomerService customerService)
        {
            _orderService = orderService;
            _cartService = cartService;
            _customerService = customerService;
        }

        public async Task<IActionResult> Index()
        {
            var customerId = await GetLoggedInCustomerIdAsync();
            var orders = await _orderService.GetOrdersByCustomerIdAsync(customerId);
            var sortedOrders = orders
                .OrderBy(o => o.Status == OrderStatus.Çatdırıldı) 
                .ThenByDescending(o => o.CreatedAt) 
                .ToList();

            return View(sortedOrders);
        }

        [HttpGet]
        public async Task<IActionResult> PlaceOrder()
        {
            return View(new PlaceOrderVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(PlaceOrderVM model)
        {
            var customerId = await GetLoggedInCustomerIdAsync();
            if (!ModelState.IsValid)
                return View(model);

            if (model.PaymentMethod == PaymentMethod.Card)
            {
                TempData["OrderData"] = JsonConvert.SerializeObject(model);
                return RedirectToAction("PayWithCard");
            }

            var result = await _orderService.PlaceOrderAsync(model, customerId);

            if (!result.Success)
            {
                TempData["Error"] = result.ErrorMessage;
                return RedirectToAction("Index", "Cart");
            }

            return RedirectToAction("Index", "Order");
        }


        [HttpGet]
        public async Task<IActionResult> PayWithCard()
        {
            var customerId = await GetLoggedInCustomerIdAsync();

            var cart = await _cartService.GetCartAsync(customerId);
            if (cart == null || !cart.Items.Any())
                return RedirectToAction("Index", "Cart");

            var modelJson = TempData["OrderData"]?.ToString();
            if (string.IsNullOrEmpty(modelJson))
                return RedirectToAction("PlaceOrder");

            var model = JsonConvert.DeserializeObject<PlaceOrderVM>(modelJson);

            string successUrl = Url.Action("StripeSuccess", "Order", null, Request.Scheme);
            string cancelUrl = Url.Action("PlaceOrder", "Order", null, Request.Scheme);
            return RedirectToAction("CreateCheckoutSession", "Stripe", new
            {
                amount = cart.TotalPrice,
                successUrl,
                cancelUrl
            });
        }

        [HttpGet]
        public async Task<IActionResult> StripeSuccess()
        {
            var customerId = await GetLoggedInCustomerIdAsync();
            var modelJson = TempData["OrderData"]?.ToString();

            if (string.IsNullOrEmpty(modelJson))
                return RedirectToAction("Index");

            var model = JsonConvert.DeserializeObject<PlaceOrderVM>(modelJson);

            await _orderService.PlaceOrderAsync(model, customerId);
            await _cartService.ClearCartAsync(customerId);
            return RedirectToAction("Index", "Order");
        }

        public IActionResult OrderConfirmation()
        {
            return View();
        }

        private async Task<int> GetLoggedInCustomerIdAsync()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _customerService.GetByUserIdAsync(userId);

            if (customer == null)
                throw new Exception("Customer not found.");

            return customer.Id;
        }


    }
}
