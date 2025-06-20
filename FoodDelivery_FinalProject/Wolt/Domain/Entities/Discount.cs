using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Discount : BaseEntity
    {
        public string Name { get; set; }
        public decimal DiscountPercentage { get; set; }
        public bool IsActive { get; set; }
        public DateTime? EndDate { get; set; }

        public ICollection<MenuItemDiscounts> MenuItemDiscounts { get; set; }

    }
}
