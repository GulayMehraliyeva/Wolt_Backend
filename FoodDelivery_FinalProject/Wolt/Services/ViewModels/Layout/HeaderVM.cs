using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.Layout
{
    public class HeaderVM
    {
        public string UserFullName;
        public string UserAddress;
        public Dictionary<string, string> Settings { get; set; }
    }
}
