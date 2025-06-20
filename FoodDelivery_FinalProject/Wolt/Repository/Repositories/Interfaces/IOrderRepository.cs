using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Interfaces
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<List<Order>> GetOrdersByCustomerIdAsync(int customerId);
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<List<Order>> GetAllDetailedAsync();
        Task<Order> GetOrderByIdDetailedAsync(int id);
        Task<List<Order>> GetOrdersByCourierIdAsync(int courierId);
    }
}
