using Domain.Helpers.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.Order
{
    public class OrderEditVM
    {
        public int OrderId { get; set; }
        public OrderStatus Status { get; set; }
        public int? CourierId { get; set; }
        public DateTime? EstimatedDeliveryTime { get; set; }
        public List<SelectListItem> Couriers { get; set; }
    }
}
