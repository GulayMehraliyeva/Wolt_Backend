using Domain.Entities;
using Service.ViewModels.Courier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface ICourierService
    {
        Task CourierRegister(CourierRegisterVM request);
        Task<IEnumerable<CourierVM>> GetAllCourierAsync();
        Task<List<CourierAssignVM>> GetCouriersForAssignmentAsync();
        Task<CourierDetailVM> GetByUserIdAsync(string userId);
        Task DeleteByUserIdAsync(string userId);
    }
}
