using Service.ViewModels.Discount;
using Service.ViewModels.Restaurant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface IDiscountService
    {
        Task CreateAsync(DiscountCreateVM request);
        Task<IEnumerable<DiscountVM>> GetAllAsync();
        Task<DiscountVM> GetByIdAsync(int id);

        Task EditAsync(int id, DiscountEditVM editVm);

        Task DeleteAsync(int id);
    }
}
