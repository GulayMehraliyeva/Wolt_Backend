using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.RestaurantReview
{
    public class RestaurantReviewVM
    {
        public int Id { get; set; }

        public int RestaurantId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public string CustomerName { get; set; }
        public Domain.Entities.Customer Customer { get; set; }
        public int CustomerId { get; set; }

    }
}
