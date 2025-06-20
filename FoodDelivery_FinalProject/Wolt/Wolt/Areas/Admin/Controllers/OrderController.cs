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
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ICourierService courierService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _courierService = courierService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("OrderController: Index method used");
            var orders = await _orderService.GetAllOrdersAsync();
            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("OrderController: Edit GET method used for OrderId={OrderId}", id);

            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning("OrderController: Edit GET - Order not found for OrderId={OrderId}", id);
                return NotFound();
            }

            if (order.Status == OrderStatus.Çatdırıldı)
            {
                _logger.LogWarning("OrderController: Edit GET - Attempt to edit delivered order OrderId={OrderId}", id);
                return BadRequest("You cannot edit a delivered order.");
            }

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

        [HttpPost]
        public async Task<IActionResult> Edit(OrderEditVM model)
        {
            _logger.LogInformation("OrderController: Edit POST method used for OrderId={OrderId}", model.OrderId);

            await _orderService.UpdateOrderStatusAndCourierAsync(model.OrderId, model.Status, model.CourierId, model.EstimatedDeliveryTime);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            _logger.LogInformation("OrderController: Detail method used for OrderId={OrderId}", id);

            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning("OrderController: Detail - Order not found for OrderId={OrderId}", id);
                return NotFound();
            }

            return View(order);
        }
    }



}

