﻿using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class RestaurantCategory : BaseEntity
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public ICollection<Restaurant> Restaurants { get; set; }
    }
}
