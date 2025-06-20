using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service.Services;
using Service.Services.Interfaces;
using Service.ViewModels.Courier;

namespace Wolt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class CourierController : Controller
    {
        private readonly ICourierService _courierService;
        private readonly ILogger<CourierController> _logger;
        private readonly IAccountService _accountService;
        private readonly UserManager<AppUser> _userManager;


        public CourierController(ICourierService courierService, 
                                 ILogger<CourierController> logger,
                                 IAccountService accountService,
                                 UserManager<AppUser> userManager)
        {
            _courierService = courierService;
            _logger = logger;
            _accountService = accountService;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var couriers = await _courierService.GetAllCourierAsync();

            foreach (var courier in couriers)
            {
                var user = await _userManager.FindByIdAsync(courier.Id);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    courier.Roles = roles;
                }
                else
                {
                    courier.Roles = new List<string>();
                }
            }

            return View(couriers);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> CourierRegister()
        {
            _logger.LogInformation("-----------------------------------------------------------------------------------");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CourierRegister(CourierRegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _courierService.CourierRegister(model);
                return RedirectToAction("Index", "Courier");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Detail(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return BadRequest();

            var courier = await _courierService.GetByUserIdAsync(userId);
            if (courier == null) return NotFound();

            return View(courier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string userId)
        {
            await _courierService.DeleteByUserIdAsync(userId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeAdmin(string userId)
        {
            try
            {
                await _accountService.AddAdminRoleAsync(userId);
                TempData["Success"] = "User promoted to Admin.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAdmin(string userId)
        {
            try
            {
                await _accountService.RemoveAdminRoleAsync(userId);
                TempData["Success"] = "Admin role removed from user.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
        }



    }
}
