using Domain.Entities;
using Service.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface IAccountService
    {
        Task Register(RegisterVM request);
        Task<bool> Login(LoginVM loginVM);
        Task Logout();
        Task<AppUser> GetCurrentUserAsync();
        Task<bool> ResetPasswordAsync(ResetPasswordVM model);
        Task ForgotPasswordAsync(ForgotPasswordVM model);
        Task AddAdminRoleAsync(string userId);
        Task RemoveAdminRoleAsync(string userId);
    }
}
