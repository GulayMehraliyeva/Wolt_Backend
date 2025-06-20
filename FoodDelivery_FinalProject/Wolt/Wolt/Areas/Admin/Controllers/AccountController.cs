using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service.Helpers.Enums;
using Service.Services;
using Service.Services.Interfaces;
using Service.ViewModels.Account;

namespace Wolt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class AccountController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAccountService _accountService;

        public AccountController(RoleManager<IdentityRole> roleManager,
                                 IAccountService accountService)
        {
            _roleManager = roleManager;
            _accountService = accountService;
        }



        //[HttpGet]
        //public async Task<IActionResult> CreateRolesAsync()
        //{
        //    foreach (var item in Enum.GetValues(typeof(Roles)))
        //    {
        //        if (!await _roleManager.RoleExistsAsync(item.ToString()))
        //        {
        //            await _roleManager.CreateAsync(new IdentityRole { Name = item.ToString() });
        //        }


        //    }

        //    return Ok();
        //}
    }
}
