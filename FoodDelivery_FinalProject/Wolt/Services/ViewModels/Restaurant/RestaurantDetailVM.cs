using Service.ViewModels.RestaurantReview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.Restaurant
{
    public class RestaurantDetailVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string Image { get; set; }

        public int EstimatedDeliveryTime { get; set; }

        public decimal DeliveryCost { get; set; }

        public string CategoryName { get; set; }

        public List<RestaurantReviewVM> Reviews { get; set; }
        public int RestaurantCategoryId { get; set; }
        public double AverageRating { get; set; }
    }
}
