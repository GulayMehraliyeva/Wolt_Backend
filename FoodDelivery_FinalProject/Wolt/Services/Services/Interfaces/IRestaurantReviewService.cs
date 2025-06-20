using Service.ViewModels.Discount;
using Service.ViewModels.RestaurantReview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface IRestaurantReviewService
    {
        Task CreateAsync(RestaurantReviewCreateVM request);
        Task<IEnumerable<RestaurantReviewVM>> GetAllAsync();
        Task<RestaurantReviewCreateVM> GetByIdAsync(int id);
        Task DeleteAsync(int id);
        Task<IEnumerable<RestaurantReviewVM>> GetAllByRestaurantIdAsync(int restaurantId);

    }
}
