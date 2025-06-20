using AutoMapper;
using Domain.Entities;
using Domain.Helpers.Enums;
using Repository.Repositories;
using Repository.Repositories.Interfaces;
using Service.Helpers;
using Service.Helpers.Results;
using Service.Services.Interfaces;
using Service.ViewModels.Courier;
using Service.ViewModels.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICourierRepository _courierRepository;
        private readonly IMapper _mapper;

        public OrderService(
            ICartRepository cartRepository,
            IOrderRepository orderRepository,
            ICustomerRepository customerRepository,
            IMapper mapper,
            ICourierRepository courierRepository)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _mapper = mapper;
            _courierRepository = courierRepository;
        }

        public async Task<OrderResult> PlaceOrderAsync(PlaceOrderVM model, int customerId)
        {
            var cart = await _cartRepository.GetCartWithItemsAsync(customerId);

            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                return new OrderResult { Success = false, ErrorMessage = "Səbət boşdur." };

            var orderItems = new List<OrderItem>();
            decimal totalPrice = 0;

            foreach (var cartItem in cart.CartItems)
            {
                var menuItem = cartItem.MenuItem;

                var discounts = menuItem.MenuItemDiscounts?.Select(d => d.Discount.DiscountPercentage) ?? Enumerable.Empty<decimal>();
                var discountedPrice = DiscountCalculator.ApplyDiscounts(menuItem.Price, discounts);

                orderItems.Add(new OrderItem
                {
                    MenuItemId = menuItem.Id,
                    Quantity = cartItem.Quantity
                });

                totalPrice += cartItem.Quantity * discountedPrice;
            }

            decimal deliveryCost = 0;

            var firstMenuItem = cart.CartItems.FirstOrDefault()?.MenuItem;
            if (firstMenuItem?.Category?.Restaurant != null)
            {
                deliveryCost = firstMenuItem.Category.Restaurant.DeliveryCost;
            }

            totalPrice += deliveryCost;

            var order = new Order
            {
                CustomerId = customerId,
                CourierId = null,
                DeliveryAddress = model.DeliveryAddress,
                Notes = model.Notes,
                PaymentMethod = model.PaymentMethod,
                Status = OrderStatus.Gözləyir,
                TotalPrice = totalPrice,
                CreatedDate = DateTime.UtcNow,
                OrderItems = orderItems
            };

            await _orderRepository.CreateAsync(order);
            await _cartRepository.ClearCartAsync(customerId);

            return new OrderResult { Success = true };
        }



        public async Task<List<OrderVM>> GetOrdersByCustomerIdAsync(int customerId)
        {
            var orders = await _orderRepository.GetOrdersByCustomerIdAsync(customerId);

            return orders.Select(order =>
            {
                var firstMenuItem = order.OrderItems.FirstOrDefault()?.MenuItem;
                decimal deliveryCost = 0;
                if (firstMenuItem?.Category?.Restaurant != null)
                {
                    deliveryCost = firstMenuItem.Category.Restaurant.DeliveryCost;
                }

                return new OrderVM
                {
                    Id = order.Id,
                    CustomerName = order.Customer?.User?.FullName,
                    DeliveryAddress = order.DeliveryAddress,
                    Notes = order.Notes,
                    PaymentMethod = order.PaymentMethod,
                    Status = order.Status,
                    TotalPrice = order.TotalPrice,
                    DeliveryCost = deliveryCost,   
                    CreatedAt = order.CreatedDate,
                    RestaurantName = firstMenuItem?.Category?.Restaurant?.Name ?? "Unknown",
                    CourierName = order.Courier?.User?.FullName,
                    CourierPhoneNumber = order.Courier?.User?.PhoneNumber,
                    DeliveredAt = order.DeliveredAt,
                    EstimatedDeliveryTime = order.EstimatedDeliveryTime,
                    Items = order.OrderItems.Select(oi => new OrderItemVM
                    {
                        MenuItemName = oi.MenuItem.Name,
                        Price = oi.MenuItem.Price,
                        Quantity = oi.Quantity,
                        Image = oi.MenuItem.Image,
                    }).ToList()
                };
            }).ToList();
        }

        public async Task<List<OrderVM>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllDetailedAsync();

            return orders.Select(order => new OrderVM
            {
                Id = order.Id,
                CustomerName = order.Customer?.User?.FullName,
                DeliveryAddress = order.DeliveryAddress,
                Notes = order.Notes,
                PaymentMethod = order.PaymentMethod,
                Status = order.Status,
                TotalPrice = order.TotalPrice,
                CreatedAt = order.CreatedDate,
                RestaurantName = order.OrderItems.FirstOrDefault()?.MenuItem?.Category?.Restaurant?.Name ?? "Unknown",
                CourierName = order.Courier?.User?.FullName,
                CourierPhoneNumber = order.Courier?.User?.PhoneNumber,
                CourierId = order.CourierId, 
                EstimatedDeliveryTime = order.EstimatedDeliveryTime,
                DeliveredAt = order.DeliveredAt,
                Items = order.OrderItems.Select(oi => new OrderItemVM
                {
                    MenuItemName = oi.MenuItem.Name,
                    Price = oi.MenuItem.Price,
                    Quantity = oi.Quantity,
                    Image = oi.MenuItem.Image,
                }).ToList()
            }).ToList();
        }

        public async Task UpdateOrderStatusAndCourierAsync(int orderId, OrderStatus status, int? courierId, DateTime? estimatedDeliveryTime)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) throw new Exception("Sifariş tapılmadı");

            if (order.CourierId.HasValue && order.CourierId != courierId)
            {
                var previousCourier = await _courierRepository.GetByIdAsync(order.CourierId.Value);
                if (previousCourier != null)
                {
                    previousCourier.IsAvailable = true;
                    await _courierRepository.UpdateAsync(previousCourier);
                }
            }

            order.Status = status;
            order.CourierId = courierId;
            order.EstimatedDeliveryTime = estimatedDeliveryTime;

            if (status == OrderStatus.Çatdırıldı)
            {
                order.DeliveredAt = DateTime.UtcNow;

                if (order.CourierId.HasValue)
                {
                    var courier = await _courierRepository.GetByIdAsync(order.CourierId.Value);
                    if (courier != null)
                    {
                        courier.IsAvailable = true;
                        await _courierRepository.UpdateAsync(courier);
                    }
                }
            }
            else if (courierId.HasValue)
            {
                var courier = await _courierRepository.GetByIdAsync(courierId.Value);
                if (courier != null)
                {
                    courier.IsAvailable = false;
                    await _courierRepository.UpdateAsync(courier);
                }
            }

            await _orderRepository.UpdateAsync(order);
        }

        public async Task<OrderVM> GetByIdAsync(int orderId)
        {
            var order = await _orderRepository.GetOrderByIdDetailedAsync(orderId); // Assumes it includes OrderItems, MenuItem -> Category -> Restaurant, Courier, Customer

            if (order == null) return null;

            var firstMenuItem = order.OrderItems.FirstOrDefault()?.MenuItem;

            decimal deliveryCost = 0;
            if (firstMenuItem?.Category?.Restaurant != null)
            {
                deliveryCost = firstMenuItem.Category.Restaurant.DeliveryCost;
            }

            return new OrderVM
            {
                Id = order.Id,
                CustomerName = order.Customer?.User?.FullName,
                CustomerPhoneNumber = order.Customer?.User?.PhoneNumber,
                DeliveryAddress = order.DeliveryAddress,
                Notes = order.Notes,
                PaymentMethod = order.PaymentMethod,
                Status = order.Status,
                TotalPrice = order.TotalPrice,
                DeliveryCost = deliveryCost,
                CreatedAt = order.CreatedDate,
                RestaurantName = firstMenuItem?.Category?.Restaurant?.Name ?? "Unknown",
                CourierName = order.Courier?.User?.FullName,
                CourierPhoneNumber = order.Courier?.User?.PhoneNumber,
                CourierId = order.CourierId,
                EstimatedDeliveryTime = order.EstimatedDeliveryTime,
                DeliveredAt = order.DeliveredAt,
                Items = order.OrderItems.Select(oi => new OrderItemVM
                {
                    MenuItemName = oi.MenuItem.Name,
                    Price = oi.MenuItem.Price,
                    Quantity = oi.Quantity,
                    Image = oi.MenuItem.Image,
                }).ToList()
            };
        }
        public async Task<List<OrderVM>> GetOrdersByCourierIdAsync(int courierId)
        {
            var orders = await _orderRepository.GetOrdersByCourierIdAsync(courierId);

            return orders.Select(order =>
            {
                var firstMenuItem = order.OrderItems.FirstOrDefault()?.MenuItem;

                decimal deliveryCost = 0;
                string restaurantName = "Unknown";

                if (firstMenuItem?.Category?.Restaurant != null)
                {
                    deliveryCost = firstMenuItem.Category.Restaurant.DeliveryCost;
                    restaurantName = firstMenuItem.Category.Restaurant.Name;
                }

                return new OrderVM
                {
                    Id = order.Id,
                    DeliveryAddress = order.DeliveryAddress,
                    Notes = order.Notes,
                    PaymentMethod = order.PaymentMethod,
                    Status = order.Status,
                    TotalPrice = order.TotalPrice,
                    CreatedAt = order.CreatedDate,
                    EstimatedDeliveryTime = order.EstimatedDeliveryTime,
                    DeliveredAt = order.DeliveredAt,
                    DeliveryCost = deliveryCost,
                    RestaurantName = restaurantName,
                    CourierId = order.CourierId,
                    CourierName = order.Courier?.User?.FullName,
                    CourierPhoneNumber = order.Courier?.User?.PhoneNumber,
                    CustomerName = order.Customer?.User?.FullName,
                    CustomerPhoneNumber = order.Customer?.User?.PhoneNumber,
                    Items = order.OrderItems.Select(oi => new OrderItemVM
                    {
                        MenuItemName = oi.MenuItem.Name,
                        Quantity = oi.Quantity,
                        Price = oi.MenuItem.Price,
                        Image = oi.MenuItem.Image
                    }).ToList()
                };
            }).ToList();
        }


    }

}
