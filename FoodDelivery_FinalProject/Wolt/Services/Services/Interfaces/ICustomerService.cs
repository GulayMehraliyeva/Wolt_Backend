using Domain.Entities;
using Service.ViewModels.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<Customer> GetByUserIdAsync(string userId);
        Task<List<CustomerVM>> GetAllCustomerVMsAsync();
        Task DeleteAsync(int id);
        Task<CustomerDetailVM> GetDetailAsync(int id);

    }
}
