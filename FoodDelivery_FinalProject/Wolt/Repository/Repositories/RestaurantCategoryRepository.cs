using Domain.Data;
using Domain.Entities;
using Repository.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class RestaurantCategoryRepository : BaseRepository<RestaurantCategory>, IRestaurantCategoryRepository
    {
        private readonly AppDbContext _context;
        public RestaurantCategoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
