using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class MenuItemDiscounts : BaseEntity
    {
        public int MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; }
        public int DiscountId { get; set; }
        public Discount Discount { get; set; }
    }
}
