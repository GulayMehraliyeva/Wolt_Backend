using Service.ViewModels.MenuItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface IMenuItemService
    {
        Task CreateAsync(MenuItemCreateVM request);

        Task<IEnumerable<MenuItemVM>> GetAllAsync();

        Task<MenuItemVM> GetByIdAsync(int id);

        Task EditAsync(MenuItemEditVM editVm);

        Task DeleteAsync(int id);

        Task<IEnumerable<MenuItemVM>> GetByCategoryIdAsync(int categoryId);
        Task<MenuItemDetailVM> DetailAsync(int id);
        Task<IEnumerable<MenuItemVM>> GetByRestaurantIdAsync(int restaurantId);

    }
}
