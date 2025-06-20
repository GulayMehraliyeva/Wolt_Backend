using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.Courier
{
    public class CourierVM
    {
        public string Id { get; set; }
        public string Email { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string VehicleType { get; set; }

        public bool IsAvailable { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
