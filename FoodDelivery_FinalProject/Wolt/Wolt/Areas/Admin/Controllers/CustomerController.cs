using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;

namespace Wolt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IAccountService _accountService;
        private readonly UserManager<AppUser> _userManager;


        public CustomerController(ICustomerService customerService, 
                                  IAccountService accountService,
                                  UserManager<AppUser> userManager)
        {
            _customerService = customerService;
            _accountService = accountService;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var customers = await _customerService.GetAllCustomerVMsAsync();

            foreach (var customer in customers)
            {
                if (!string.IsNullOrEmpty(customer.UserId))
                {
                    var user = await _userManager.FindByIdAsync(customer.UserId);
                    if (user != null)
                    {
                        customer.Roles = await _userManager.GetRolesAsync(user);
                    }
                }

                if (customer.Roles == null)
                    customer.Roles = new List<string>();
            }

            return View(customers);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Detail(int id)
        {
            var vm = await _customerService.GetDetailAsync(id);
            if (vm == null) return NotFound();

            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _customerService.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
