using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.Cart
{
    public class CartVM
    {
        public List<CartItemVM> Items { get; set; }
        public decimal TotalPrice {  get; set; }
        public string RestaurantName { get; set; }
    }
}
