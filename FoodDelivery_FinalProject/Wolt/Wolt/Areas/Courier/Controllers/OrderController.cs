using Domain.Entities;
using Domain.Helpers.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;
using Service.ViewModels.Order;

namespace Wolt.Areas.Courier.Controllers
{
    [Area("Courier")]
    [Authorize(Roles = "Courier")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICourierRepository _courierRepository;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(IOrderService orderService, ICourierRepository courierRepository, UserManager<AppUser> userManager)
        {
            _orderService = orderService;
            _courierRepository = courierRepository;
            _userManager = userManager;
        }

        [Authorize(Roles = "Courier")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var courier = await _courierRepository.GetByUserIdAsync(user.Id);
            if (courier == null) return NotFound();

            var orders = await _orderService.GetOrdersByCourierIdAsync(courier.Id);
            return View(orders);
        }

		[HttpGet]
        [Authorize(Roles = "Courier")]
        public async Task<IActionResult> Edit(int id)
		{
			var user = await _userManager.GetUserAsync(User);
			var courier = await _courierRepository.GetByUserIdAsync(user.Id);
			if (courier == null) return Forbid();

			var order = await _orderService.GetByIdAsync(id);
			if (order == null || order.CourierId != courier.Id) return NotFound();

			var model = new OrderEditVM
			{
				OrderId = order.Id,
				Status = order.Status
			};

			return View(model);
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Courier")]
        public async Task<IActionResult> UpdateStatus(OrderEditVM model)
        {
            var user = await _userManager.GetUserAsync(User);
            var courier = await _courierRepository.GetByUserIdAsync(user.Id);
            if (courier == null) return Forbid();

            var order = await _orderService.GetByIdAsync(model.OrderId);
            if (order == null || order.CourierId != courier.Id) return NotFound();

            await _orderService.UpdateOrderStatusAndCourierAsync(
                model.OrderId,
                model.Status,
                courier.Id,
                order.EstimatedDeliveryTime
            );

            return RedirectToAction(nameof(Index));
        }
    }
}
