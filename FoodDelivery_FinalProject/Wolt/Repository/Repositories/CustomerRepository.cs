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
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        private readonly AppDbContext _context;
        public CustomerRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Customer> GetByUserIdAsync(string userId)
        {
            return await _context.Customers
                .Include(c => c.User) 
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<List<Customer>> GetAllWithUsersAsync()
        {
            return await _context.Customers
                                 .Include(c => c.User)
                                 .ToListAsync();
        }
        public async Task<Customer> GetByIdWithAllDetailsAsync(int id)
        {
            return await _context.Customers
     .Include(c => c.User)
     .Include(c => c.Orders)
         .ThenInclude(o => o.Courier)
             .ThenInclude(courier => courier.User) 
     .Include(c => c.Orders)
         .ThenInclude(o => o.OrderItems)
             .ThenInclude(oi => oi.MenuItem)
                 .ThenInclude(mi => mi.Category)
                     .ThenInclude(c => c.Restaurant)
     .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
