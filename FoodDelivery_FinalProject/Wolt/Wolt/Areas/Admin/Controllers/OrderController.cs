using Domain.Helpers.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service.Services.Interfaces;
using Service.ViewModels.Order;

namespace Wolt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICourierService _courierService;

        public OrderController(IOrderService orderService, ICourierService courierService)
        {
            _orderService = orderService;
            _courierService = courierService;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return View(orders);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {

            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            if (order.Status == OrderStatus.Çatdırıldı)
                return BadRequest("You cannot edit a delivered order.");

            var couriers = await _courierService.GetCouriersForAssignmentAsync();

            var filteredCouriers = couriers
                .Where(c => c.IsAvailable || c.Id == order.CourierId)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.FullName
                }).ToList();

            var vm = new OrderEditVM
            {
                OrderId = order.Id,
                Status = order.Status,
                CourierId = order.CourierId,
                EstimatedDeliveryTime = order.EstimatedDeliveryTime,
                Couriers = filteredCouriers
            };

            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(OrderEditVM model)
        {
            await _orderService.UpdateOrderStatusAndCourierAsync(model.OrderId, model.Status, model.CourierId, model.EstimatedDeliveryTime);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Detail(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            return View(order);
        }

    }


}

