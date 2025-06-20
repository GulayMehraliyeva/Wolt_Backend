using Service.ViewModels.MenuCategory;
using Service.ViewModels.Slider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface ISliderService
    {
        Task CreateAsync(SliderCreateVM request);
        Task<IEnumerable<SliderVM>> GetAllAsync();
        Task<SliderVM> GetByIdAsync(int id);
        Task EditAsync(int id, SliderEditVM editVm);
        Task DeleteAsync(int id);
    }
}
