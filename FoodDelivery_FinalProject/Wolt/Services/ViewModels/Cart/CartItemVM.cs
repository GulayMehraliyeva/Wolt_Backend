﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.Cart
{
    public class CartItemVM
    {
        public int Id { get; set; }
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountedPrice { get; set; }

        public int Quantity { get; set; }
        public string Image { get; set; }
        public string RestaurantName { get; set; }
        public string Description { get; set; }

    }
}
