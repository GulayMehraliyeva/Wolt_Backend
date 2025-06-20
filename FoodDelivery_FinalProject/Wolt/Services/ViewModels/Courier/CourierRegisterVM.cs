using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.Courier
{
    public class CourierRegisterVM
    {
        public string Email { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string VehicleType { get; set; }

        public bool IsAvailable { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
