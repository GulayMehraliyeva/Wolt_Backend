using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class RestaurantReview : BaseEntity
    {
        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int Rating { get; set; }

        public string Comment { get; set; }
    }
}
