using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.RestaurantReview
{
    public class RestaurantReviewCreateVM
    {
        public int RestaurantId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [MaxLength(200)]
        public string Comment { get; set; }
    }
}
