using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;
using Service.ViewModels.Profile;

namespace Wolt.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;
        private readonly UserManager<AppUser> _userManager;

        public ProfileController(IProfileService profileService, UserManager<AppUser> userManager)
        {
            _profileService = profileService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string message = null, string error = null)
        {
            var profile = await _profileService.GetCurrentUserProfileAsync();

            if (!string.IsNullOrEmpty(message))
                ViewData["Message"] = message;

            if (!string.IsNullOrEmpty(error))
                ViewData["Error"] = error;

            return View(profile);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                return RedirectToAction("Index", new { error = "Yeni şifrə və təkrar şifrə uyğun deyil." });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", new { error = "İstifadəçi tapılmadı." });
            }

            var success = await _profileService.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!success)
            {
                return RedirectToAction("Index", new { error = "Şifrə səhvdir və ya yeni şifrə uyğun deyil." });
            }

            return RedirectToAction("Index", new { message = "Şifrə uğurla dəyişdirildi." });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEmail(string newEmail)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", new { error = "İstifadəçi tapılmadı." });
            }

            try
            {
                await _profileService.UpdateEmailAsync(user, newEmail);
                return RedirectToAction("Index", new { message = "Email uğurla dəyişdirildi." });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePhoneNumber(string newPhoneNumber)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", new { error = "İstifadəçi tapılmadı." });
            }

            try
            {
                await _profileService.UpdatePhoneNumberAsync(user, newPhoneNumber);
                return RedirectToAction("Index", new { message = "Telefon nömrəsi uğurla dəyişdirildi." });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", new { error = ex.Message });
            }
        }

		[HttpPost]
		public async Task<IActionResult> UpdateAddress(string newAddress)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return RedirectToAction("Index", new { error = "İstifadəçi tapılmadı." });
			}

			try
			{
				await _profileService.UpdateAddressAsync(user.Id, newAddress);
				return RedirectToAction("Index", new { message = "Ünvan uğurla dəyişdirildi." });
			}
			catch (Exception ex)
			{
				return RedirectToAction("Index", new { error = ex.Message });
			}
		}

	}
}
