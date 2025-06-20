using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Courier : BaseEntity
    {
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string VehicleType { get; set; }

        public bool IsAvailable { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
