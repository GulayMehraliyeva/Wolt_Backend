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
    public class MenuItemRepository : BaseRepository<MenuItem>, IMenuItemRepository
    {
        private readonly AppDbContext _context;
        public MenuItemRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<MenuItem> GetWithCategoryAsync(int menuItemId)
        {
            return await _context.MenuItems
                .Include(mi => mi.Category)
                    .ThenInclude(c => c.Restaurant)
                .FirstOrDefaultAsync(mi => mi.Id == menuItemId);
        }
    }
}
