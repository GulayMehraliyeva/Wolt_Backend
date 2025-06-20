using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Customer : BaseEntity
    {
        public string UserId { get; set; }  
        public AppUser User { get; set; }

        public string Address { get; set; }     
        public ICollection<Order> Orders { get; set; }
        public ICollection<RestaurantReview> Reviews { get; set; }

    }
}
