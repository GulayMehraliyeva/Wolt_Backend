using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Interfaces
{
    public interface ICartRepository : IBaseRepository<Cart>
    {
        Task<Cart> GetCartWithItemsAsync(int customerId);
        Task AddCartItemAsync(CartItem cartItem);
        Task UpdateCartItemAsync(CartItem cartItem);
        Task ClearCartAsync(int customerId);
        Task<CartItem> GetCartItemByIdAsync(int cartItemId);
        Task RemoveCartItemAsync(CartItem cartItem);

    }
}
