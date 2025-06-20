using Domain.Helpers.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.Order
{
    public class PlaceOrderVM
    {
        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Required(ErrorMessage = "Delivery address is required")]
        public string DeliveryAddress { get; set; }

        public string Notes { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
