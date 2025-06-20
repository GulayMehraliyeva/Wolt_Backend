using Domain.Entities;
using Service.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface ICartService
    {
        Task<List<CartItemVM>> GetCartItemsAsync(int customerId);
        Task AddToCartAsync(int customerId, int menuItemId);
        Task ClearCartAsync(int customerId);
        Task<CartVM> GetCartAsync(int customerId);
        Task UpdateCartItemQuantityAsync(int cartItemId, int quantity);
        Task RemoveCartItemAsync(int cartItemId);

    }
}
