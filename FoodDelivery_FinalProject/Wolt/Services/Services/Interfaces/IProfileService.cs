using Domain.Entities;
using Service.ViewModels.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface IProfileService
    {
        Task<ProfileVM> GetCurrentUserProfileAsync();
        Task<bool> ChangePasswordAsync(AppUser user, string currentPassword, string newPassword);
        Task UpdatePhoneNumberAsync(AppUser user, string newPhoneNumber);
        Task UpdateEmailAsync(AppUser user, string newEmail);
		Task UpdateAddressAsync(string userId, string newAddress);

	}
}
