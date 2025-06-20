using Domain.Helpers.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.Order
{
    public class OrderVM
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string DeliveryAddress { get; set; }
        public string Notes { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal DeliveryCost { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemVM> Items { get; set; }
        public string RestaurantName { get; set; }
        public string CourierName { get; set; }
        public int? CourierId { get; set; }
        public string CourierPhoneNumber { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? EstimatedDeliveryTime { get; set; }

    }

}
