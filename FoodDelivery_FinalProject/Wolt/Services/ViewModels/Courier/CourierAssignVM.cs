﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.Courier
{
    public class CourierAssignVM
    {
        public int Id { get; set; }          
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsAvailable { get; set; }
    }
}
