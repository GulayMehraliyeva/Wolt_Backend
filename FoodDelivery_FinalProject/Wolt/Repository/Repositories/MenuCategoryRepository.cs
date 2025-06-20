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
    public class MenuCategoryRepository : BaseRepository<MenuItemCategory>, IMenuCategoryRepository
    {
        private readonly AppDbContext _context;
        public MenuCategoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
