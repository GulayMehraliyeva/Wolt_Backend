using Service.ViewModels.MenuCategory;
using Service.ViewModels.Restaurant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface IMenuCategoryService
    {
        Task CreateAsync(MenuCategoryCreateVM request);
        Task<IEnumerable<MenuCategoryVM>> GetAllAsync();
        Task<MenuCategoryVM> GetByIdAsync(int id);

        Task EditAsync(int id, MenuCategoryEditVM editVm);

        Task DeleteAsync(int id);
        Task<IEnumerable<MenuCategoryVM>> GetByRestaurantIdAsync(int restaurantId);

    }
}
