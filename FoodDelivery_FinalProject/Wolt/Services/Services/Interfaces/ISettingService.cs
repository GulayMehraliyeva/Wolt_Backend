using Service.ViewModels.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface ISettingService
    {
        Task<IEnumerable<SettingVM>> GetAllAsync();
        Task<SettingEditVM> GetByIdAsync(int id);
        Task CreateAsync(SettingCreateVM vm);
        Task UpdateAsync(SettingEditVM vm);
        Task DeleteAsync(int id);
    }
}
