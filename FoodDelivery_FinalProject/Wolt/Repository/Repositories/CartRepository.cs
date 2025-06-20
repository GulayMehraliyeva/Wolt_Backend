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
    public class CartRepository : BaseRepository<Cart>, ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Cart> GetCartWithItemsAsync(int customerId)
        {
            return await _context.Carts
        .Include(c => c.CartItems)
            .ThenInclude(ci => ci.MenuItem)
                .ThenInclude(mi => mi.MenuItemDiscounts)
                    .ThenInclude(mid => mid.Discount)
        .Include(c => c.CartItems)
            .ThenInclude(ci => ci.MenuItem)
                .ThenInclude(mi => mi.Category)
                    .ThenInclude(c => c.Restaurant)
        .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task AddCartItemAsync(CartItem cartItem)
        {
            await _context.CartItems.AddAsync(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartItemAsync(CartItem cartItem)
        {
            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task ClearCartAsync(int customerId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<CartItem> GetCartItemByIdAsync(int cartItemId)
        {
            return await _context.CartItems
                .Include(ci => ci.MenuItem)
                .ThenInclude(mi => mi.Category)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId);
        }

        public async Task RemoveCartItemAsync(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }
    }

}
