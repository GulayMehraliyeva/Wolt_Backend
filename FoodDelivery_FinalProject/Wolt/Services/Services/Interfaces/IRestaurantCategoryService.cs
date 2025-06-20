using Service.ViewModels.RestaurantCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface IRestaurantCategoryService
    {
        Task CreateAsync(RestaurantCategoryCreateVM request);
        Task<IEnumerable<RestaurantCategoryVM>> GetAllAsync();
        Task<RestaurantCategoryVM> GetByIdAsync(int id);

        Task EditAsync(int id, RestaurantCategoryEditVM editVm);

        Task DeleteAsync(int id);
    }
}
