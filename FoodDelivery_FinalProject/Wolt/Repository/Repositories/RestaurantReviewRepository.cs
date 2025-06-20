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
    public class RestaurantReviewRepository : BaseRepository<RestaurantReview>, IRestaurantReviewRepository
    {
        private readonly AppDbContext _context;
        public RestaurantReviewRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RestaurantReview>> GetAllByRestaurantIdAsync(int restaurantId)
        {
            return await _context.RestaurantReviews
                .Where(r => r.RestaurantId == restaurantId)
                .Include(r => r.Customer)
                    .ThenInclude(c => c.User)
                .ToListAsync();
        }
    }
}
