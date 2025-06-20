using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Restaurant : BaseEntity
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone {  get; set; }
        public string Image { get; set; }
        public int EstimatedDeliveryTime { get; set; }
        public decimal DeliveryCost { get; set; }
        public int RestaurantCategoryId { get; set; }
        public RestaurantCategory RestaurantCategory { get; set; }
        public ICollection<RestaurantReview> Reviews { get; set; }
        public ICollection<MenuItemCategory> MenuItemCategories { get; set; }
    }
}
