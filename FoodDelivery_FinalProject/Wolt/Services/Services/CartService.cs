using AutoMapper;
using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.Helpers;
using Service.Services.Interfaces;
using Service.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IMapper _mapper;

        public CartService(ICartRepository cartRepo, IMenuItemRepository menuItemRepo, IMapper mapper)
        {
            _cartRepository = cartRepo;
            _menuItemRepository = menuItemRepo;
            _mapper = mapper;
        }

        public async Task AddToCartAsync(int customerId, int menuItemId)
        {
            var cart = await _cartRepository.GetCartWithItemsAsync(customerId);

            if (cart == null)
            {
                cart = new Cart
                {
                    CustomerId = customerId,
                    CartItems = new List<CartItem>()
                };
                await _cartRepository.CreateAsync(cart);
            }

            var menuItem = await _menuItemRepository.GetWithCategoryAsync(menuItemId);

            if (cart.CartItems.Any())
            {
                var existingRestaurantId = cart.CartItems
                    .Select(ci => ci.MenuItem.Category.RestaurantId)
                    .Distinct()
                    .Single();

                if (existingRestaurantId != menuItem.Category.RestaurantId)
                    throw new Exception("You can only order from one restaurant.");
            }

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.MenuItemId == menuItemId);
            if (existingItem != null)
            {
                existingItem.Quantity++;
                await _cartRepository.UpdateCartItemAsync(existingItem);
            }
            else
            {
                await _cartRepository.AddCartItemAsync(new CartItem
                {
                    CartId = cart.Id,
                    MenuItemId = menuItemId,
                    Quantity = 1
                });
            }
        }

        public async Task<List<CartItemVM>> GetCartItemsAsync(int customerId)
        {
            var cart = await _cartRepository.GetCartWithItemsAsync(customerId);

            if (cart == null || cart.CartItems == null)
                return new List<CartItemVM>();

            return _mapper.Map<List<CartItemVM>>(cart.CartItems.ToList());
        }

        public async Task ClearCartAsync(int customerId)
        {
            await _cartRepository.ClearCartAsync(customerId);
        }

        public async Task<CartVM> GetCartAsync(int customerId)
        {
            var cart = await _cartRepository.GetCartWithItemsAsync(customerId);

            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            {
                return new CartVM
                {
                    Items = new List<CartItemVM>(),
                    TotalPrice = 0,
                    RestaurantName = ""
                };
            }

            var cartItems = cart.CartItems.Select(ci =>
            {
                var menuItem = ci.MenuItem;

                var discounts = menuItem.MenuItemDiscounts?.Select(d => d.Discount.DiscountPercentage) ?? Enumerable.Empty<decimal>();

                var discountedPrice = DiscountCalculator.ApplyDiscounts(menuItem.Price, discounts);

                return new CartItemVM
                {
                    Id = ci.Id,
                    MenuItemId = menuItem.Id,
                    MenuItemName = menuItem.Name,
                    Price = menuItem.Price,
                    DiscountedPrice = discountedPrice,
                    Quantity = ci.Quantity,
                    Image = menuItem.Image,
                    RestaurantName = menuItem.Category.Restaurant.Name,
                    Description = menuItem.Description
                };
            }).ToList();

            var totalPrice = cartItems.Sum(i => i.Quantity * i.DiscountedPrice);
            var restaurantName = cartItems.Select(i => i.RestaurantName).FirstOrDefault();

            return new CartVM
            {
                Items = cartItems,
                TotalPrice = totalPrice,
                RestaurantName = restaurantName
            };
        }

        public async Task UpdateCartItemQuantityAsync(int cartItemId, int quantity)
        {
            var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                await _cartRepository.UpdateCartItemAsync(cartItem);
            }
        }

        public async Task RemoveCartItemAsync(int cartItemId)
        {
            var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);
            if (cartItem != null)
            {
                await _cartRepository.RemoveCartItemAsync(cartItem);
            }
        }

    }
}
