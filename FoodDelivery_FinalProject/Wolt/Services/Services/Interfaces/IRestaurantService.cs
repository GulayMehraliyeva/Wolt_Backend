using Service.ViewModels.MenuItem;
using Service.ViewModels.Restaurant;
using Service.ViewModels.Slider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface IRestaurantService
    {
        Task CreateAsync(RestaurantCreateVM request);
        Task<IEnumerable<RestaurantVM>> GetAllAsync();
        Task<RestaurantDetailVM> GetByIdAsync(int id);
        Task EditAsync(int id, RestaurantEditVM editVm);
        Task DeleteAsync(int id);
        Task<RestaurantVMUI> GetAllPaginatedWithCategories(int page, int take = 9);
        Task<IEnumerable<RestaurantVM>> SearchRestaurantsAsync(string query);
        Task<IEnumerable<RestaurantVM>> GetFilteredAsync(RestaurantFilterVM filter);
        Task<IEnumerable<RestaurantVM>> GetByCategoryIdAsync(int categoryId);

    }
}
