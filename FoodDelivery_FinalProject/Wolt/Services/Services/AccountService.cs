using AutoMapper;
using Domain.Data;
using Domain.Entities;
using Microsoft.AspNetCore.DataProtection.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repository.Repositories;
using Repository.Repositories.Interfaces;
using Service.Helpers.Enums;
using Service.Services.Interfaces;
using Service.ViewModels.Account;
using Service.ViewModels.Discount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Service.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICourierRepository _courierRepository;
        private readonly IEmailService _emailService;
        private readonly UrlEncoder _urlEncoder;


        public AccountService(UserManager<AppUser> userManager,
                              RoleManager<IdentityRole> roleManager, 
                              AppDbContext context, 
                              SignInManager<AppUser> signInManager,
                              ICourierRepository courierRepository,
                              IHttpContextAccessor httpContextAccessor,
                              IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _signInManager = signInManager;
            _courierRepository = courierRepository;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
            _urlEncoder = UrlEncoder.Default;
        }


        public async Task AddAdminRoleAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found.");

            if (!await _roleManager.RoleExistsAsync("Admin"))
                throw new Exception("Role 'Admin' does not exist.");

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
                throw new Exception("User already has Admin role.");

            var result = await _userManager.AddToRoleAsync(user, "Admin");
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task RemoveAdminRoleAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found.");

            if (!await _roleManager.RoleExistsAsync("Admin"))
                throw new Exception("Role 'Admin' does not exist.");

            var roles = await _userManager.GetRolesAsync(user);

            if (!roles.Contains("Admin"))
                throw new Exception("User does not have Admin role.");

            bool isCustomer = roles.Contains("Customer");
            bool isCourier = roles.Contains("Courier");

            if (isCustomer && !roles.Contains("Customer"))
                throw new Exception("Customer must have 'Customer' role.");

            if (isCourier && !roles.Contains("Courier"))
                throw new Exception("Courier must have 'Courier' role.");

            if (!isCustomer && !isCourier)
                throw new Exception("User must have at least Customer or Courier role.");

            var result = await _userManager.RemoveFromRoleAsync(user, "Admin");

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }




        public async Task Register(RegisterVM request)
        {
            var user = new AppUser
            {
                UserName = request.UserName,
                Email = request.Email,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, Roles.Customer.ToString());

            var customer = new Customer
            {
                UserId = user.Id,
                Address = request.Address,
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            var userId = WebUtility.UrlEncode(user.Id);

            var requestUrl = _httpContextAccessor.HttpContext?.Request;
            var confirmationLink = $"{requestUrl.Scheme}://{requestUrl.Host}/account/confirmemail?userId={userId}&token={encodedToken}";

            var message = $"Please confirm your email by clicking this link: <a href='{confirmationLink}'>Confirm Email</a>";

            await _emailService.SendEmailAsync(user.Email, "Confirm your email", message);
        }

        public async Task<bool> Login(LoginVM loginVM)
        {
            var user = await _userManager.FindByEmailAsync(loginVM.EmailOrUsername)
                        ?? await _userManager.FindByNameAsync(loginVM.EmailOrUsername);

            if (user == null) return false;

            if (!await _userManager.IsEmailConfirmedAsync(user))
                throw new Exception("Please confirm your email before logging in.");

            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
            return result.Succeeded;
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IEnumerable<RoleVM>> GetAllRolesAsync()
        {
            List<RoleVM> roles = [];

            var dbRoles = await _roleManager.Roles.ToListAsync();

            foreach (var item in dbRoles)
            {
                var users = await _userManager.GetUsersInRoleAsync(item.ToString());

                roles.Add(new RoleVM
                {
                    Name = item.Name,
                    Users = users.Select(m => new UserVM
                    {
                        FullName = m.FullName,
                        Email = m.Email,
                        UserName = m.UserName,
                        PhoneNumber = m.PhoneNumber
                    }).ToArray()
                });
            }

            return roles;
        }

        public async Task<IEnumerable<UserVM>> GetAllUsersAsync()
        {
            List<UserVM> users = new();

            var dbUsers = await _userManager.Users.ToListAsync();

            foreach (var item in dbUsers)
            {
                var userRoles = await _userManager.GetRolesAsync(item);

                users.Add(new UserVM
                {
                    FullName = item.FullName,
                    Email = item.Email,
                    UserName = item.UserName,
                    PhoneNumber = item.PhoneNumber,
                    Roles = userRoles.ToArray()
                });
            }

            return users;
        }

        public async Task<AppUser> GetCurrentUserAsync()
        {
            var userIdStr = _httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdStr)) return null;

            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userIdStr);
        }

        public async Task ForgotPasswordAsync(ForgotPasswordVM model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                return; 

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            var userId = WebUtility.UrlEncode(user.Id);

            var requestUrl = _httpContextAccessor.HttpContext?.Request;
            var resetLink = $"{requestUrl.Scheme}://{requestUrl.Host}/account/resetpassword?userId={userId}&token={encodedToken}";

            var message = $"Reset your password by clicking this link: <a href='{resetLink}'>Reset Password</a>";
            await _emailService.SendEmailAsync(user.Email, "Reset Password", message);
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordVM model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return false;

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            return result.Succeeded;
        }
    }
}
