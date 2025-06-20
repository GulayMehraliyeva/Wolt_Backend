using Service.ViewModels.Brand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface IBrandService
    {
        Task<IEnumerable<BrandVM>> GetAllAsync();
        Task<BrandVM> GetByIdAsync(int id);
        Task CreateAsync(BrandCreateVM request);
        Task EditAsync(int id, BrandEditVM editVm);
        Task DeleteAsync(int id);
    }
}
