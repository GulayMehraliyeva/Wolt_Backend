using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.MenuItem
{
    public class MenuItemDetailVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public string RestaurantName {  get; set; }
        public string CategoryName { get; set; }
        public List<string> DiscountNames { get; set; }
    }
}
