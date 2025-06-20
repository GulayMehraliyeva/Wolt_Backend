using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;
using Service.ViewModels.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ICustomerRepository _customerRepository;

		public ProfileService(UserManager<AppUser> userManager, 
                              IHttpContextAccessor httpContextAccessor,
                              ICustomerRepository customerRepository)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _customerRepository = customerRepository;
        }

        public async Task<ProfileVM> GetCurrentUserProfileAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new Exception("User is not logged in.");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new Exception("User not found.");

            var customer = await _customerRepository.GetByUserIdAsync(user.Id);

            return new ProfileVM
            {
                UserId = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = customer?.Address
            };
        }


        public async Task<bool> ChangePasswordAsync(AppUser user, string currentPassword, string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return result.Succeeded;
        }
        public async Task UpdatePhoneNumberAsync(AppUser user, string newPhoneNumber)
        {
            user.PhoneNumber = newPhoneNumber;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task UpdateEmailAsync(AppUser user, string newEmail)
        {
            user.Email = newEmail;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

		public async Task UpdateAddressAsync(string userId, string newAddress)
		{
			var customer = await _customerRepository.GetByUserIdAsync(userId);

			if (customer == null)
				throw new Exception("Müştəri tapılmadı.");

			customer.Address = newAddress;
			await _customerRepository.UpdateAsync(customer);
		}

	}
}
