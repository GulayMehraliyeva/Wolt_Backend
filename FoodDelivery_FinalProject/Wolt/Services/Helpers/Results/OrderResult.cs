using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers.Results
{
    public class OrderResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
