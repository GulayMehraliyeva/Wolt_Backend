using Domain.Data;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Services.Interfaces;
using Service.ViewModels.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class LayoutService : ILayoutService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ISettingService _settingService;

        public LayoutService(AppDbContext context, 
                             UserManager<AppUser> userManager, 
                             ISettingService settingService)
        {
            _context = context;
            _userManager = userManager;
            _settingService = settingService;
        }

        public async Task<HeaderVM> GetHeaderDataAsync(ClaimsPrincipal userPrincipal)
        {
            AppUser user = null;
            string address = null;

            if (userPrincipal.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(userPrincipal.Identity.Name);

                if (user != null)
                {
                    var customer = await _context.Customers
                        .FirstOrDefaultAsync(c => c.UserId == user.Id);

                    address = customer?.Address;
                }
            }

            var settingsList = await _settingService.GetAllAsync();

            var settingsDict = settingsList.ToDictionary(s => s.Key, s => s.Value);

            return new HeaderVM
            {
                UserFullName = user?.FullName,
                UserAddress = address,
                Settings = settingsDict
            };
        }

        public async Task<FooterVM> GetFooterDataAsync(ClaimsPrincipal userPrincipal)
        {
            var settingsList = await _settingService.GetAllAsync();

            var settingsDict = settingsList.ToDictionary(s => s.Key, s => s.Value);

            return new FooterVM
            {
                Settings = settingsDict
            };
        }
    }
}