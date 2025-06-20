using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.Account
{
    public class RoleVM
    {
        public string Name { get; set; }
        public UserVM[] Users { get; set; }
    }
}
