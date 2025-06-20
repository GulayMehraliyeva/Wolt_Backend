using Domain.Entities.Common;
using Domain.Helpers.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Order : BaseEntity
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int? CourierId { get; set; }
        public Courier Courier { get; set; }

        public OrderStatus Status { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }

        public DateTime? DeliveredAt { get; set; }
        public DateTime? EstimatedDeliveryTime { get; set; }
        public decimal TotalPrice { get; set; }
        public string DeliveryAddress { get; set; }
        public string Notes { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
