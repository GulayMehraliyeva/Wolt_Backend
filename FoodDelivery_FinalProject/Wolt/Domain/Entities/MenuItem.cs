using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class MenuItem : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image {  get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public MenuItemCategory Category { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<MenuItemDiscounts> MenuItemDiscounts { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
