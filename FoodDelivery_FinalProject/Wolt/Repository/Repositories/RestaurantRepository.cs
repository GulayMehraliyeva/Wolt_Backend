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
    public class RestaurantRepository : BaseRepository<Restaurant>, IRestaurantRepository
    {
        private readonly AppDbContext _context;
        public RestaurantRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Restaurant>> GetAllPaginated(int page, int take)
        {
            int skip = (page - 1) * take;
            return await _context.Restaurants
                                 .Include(r => r.RestaurantCategory)
                                 .Include(r => r.Reviews)
                                 .Skip(skip)
                                 .Take(take)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Restaurant>> SearchByNameAsync(string query)
        {
            return await _context.Restaurants
                .Where(r => r.Name.Contains(query))
                .Include(r => r.RestaurantCategory)
                .ToListAsync();
        }

        public async Task<Restaurant> GetByIdWithIncludesAsync(int id, Func<IQueryable<Restaurant>, IQueryable<Restaurant>> include)
        {
            IQueryable<Restaurant> query = _context.Restaurants;
            if (include != null)
            {
                query = include(query);
            }

            return await query.FirstOrDefaultAsync(r => r.Id == id);
        }

    }
}
