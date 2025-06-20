using Domain.Entities;
using Service.ViewModels.Restaurant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.MenuItem
{
    public class MenuItemVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountedPrice { get; set; }
        public string Image { get; set; }
        public int RestaurantId { get; set; }
        public int CategoryId { get; set; }
        public MenuItemCategory MenuItemCategory { get; set; }
        public List<int> DiscountIds { get; set; } = new();
    }
}
