using Domain.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllDetailedAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                        .ThenInclude(mi => mi.Category)
                            .ThenInclude(c => c.Restaurant)
                .Include(o => o.Courier)
                    .ThenInclude(c => c.User)
                .Include(o => o.Customer)
                    .ThenInclude(c => c.User)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByCustomerIdAsync(int customerId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                        .ThenInclude(mi => mi.Category)
                            .ThenInclude(c => c.Restaurant)
                .Include(o => o.Courier)
                    .ThenInclude(c => c.User)
                .Include(o => o.Customer)
                    .ThenInclude(c => c.User)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }
        public async Task<Order> GetOrderByIdDetailedAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                        .ThenInclude(mi => mi.Category)
                            .ThenInclude(c => c.Restaurant)
                .Include(o => o.Courier)
                    .ThenInclude(c => c.User)
                .Include(o => o.Customer)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task<List<Order>> GetOrdersByCourierIdAsync(int courierId)
        {
            return await _context.Orders
                .Where(o => o.CourierId == courierId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Customer)
                    .ThenInclude(c => c.User)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }
    }
}
