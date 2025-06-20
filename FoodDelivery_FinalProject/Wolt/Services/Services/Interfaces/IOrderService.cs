using Domain.Entities;
using Domain.Helpers.Enums;
using Service.Helpers.Results;
using Service.ViewModels.Courier;
using Service.ViewModels.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResult> PlaceOrderAsync(PlaceOrderVM model, int customerId);
        Task<List<OrderVM>> GetOrdersByCustomerIdAsync(int customerId);
        Task<List<OrderVM>> GetAllOrdersAsync();
        Task UpdateOrderStatusAndCourierAsync(int orderId, OrderStatus status, int? courierId, DateTime? estimatedDeliveryTime);
        Task<OrderVM> GetByIdAsync(int orderId);
        Task<List<OrderVM>> GetOrdersByCourierIdAsync(int courierId);
    }
}
