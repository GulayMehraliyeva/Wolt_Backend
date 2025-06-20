using Domain.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class CourierRepository : BaseRepository<Courier>, ICourierRepository
    {
        private readonly AppDbContext _context;
        public CourierRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Courier> GetByUserIdAsync(string userId)
        {
            var courier = await _context.Couriers
                .Include(c => c.User)
                .Include(c => c.Orders)
                    .ThenInclude(o => o.Customer)
                        .ThenInclude(cust => cust.User)
                .Include(c => c.Orders)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.MenuItem)
                            .ThenInclude(mi => mi.Category)
                                .ThenInclude(cat => cat.Restaurant)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (courier == null)
                throw new KeyNotFoundException($"Courier not found for userId: {userId}");

            return courier;
        }


        public async Task<Courier> GetByIdWithUserAsync(int id)
        {
            return await _context.Couriers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

    }
}
