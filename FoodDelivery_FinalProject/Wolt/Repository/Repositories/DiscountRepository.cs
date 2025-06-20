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
    public class DiscountRepository : BaseRepository<Discount>, IDiscountRepository
    {
        private readonly AppDbContext _context;
        public DiscountRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
