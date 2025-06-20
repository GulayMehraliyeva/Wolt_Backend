using Service.Helpers.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.Profile
{
    public class ProfileVM
    {
        public string UserId { get; set; }          
        public string FullName { get; set; }         
        public string UserName { get; set; }        
        public string Email { get; set; }            
        public string PhoneNumber { get; set; }     

        public string Address { get; set; }

        public string VehicleType { get; set; }
        public bool IsAvailable { get; set; }
        public Roles Roles { get; set; }
    }
}
