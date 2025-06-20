using Domain.Data;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repository.Repositories;
using Repository.Repositories.Interfaces;
using Service.Helpers.Enums;
using Service.Services.Interfaces;
using Service.ViewModels.Account;
using Service.ViewModels.Courier;
using Service.ViewModels.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Service.Services
{
    public class CourierService : ICourierService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly ICourierRepository _courierRepository;
        private readonly IEmailService _emailService;
        private readonly UrlEncoder _urlEncoder;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public CourierService(UserManager<AppUser> userManager,
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
            _courierRepository = courierRepository;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            _urlEncoder = UrlEncoder.Default;

        }
        public async Task CourierRegister(CourierRegisterVM request)
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

                await _userManager.AddToRoleAsync(user, Roles.Courier.ToString());

                var courier = new Courier
                {
                    UserId = user.Id,
                    VehicleType = request.VehicleType,
                    IsAvailable = request.IsAvailable
                };

                _context.Couriers.Add(courier);
                await _context.SaveChangesAsync();

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = WebUtility.UrlEncode(token);
                var userId = WebUtility.UrlEncode(user.Id);

                var requestUrl = _httpContextAccessor.HttpContext?.Request;
                var confirmationLink = $"{requestUrl.Scheme}://{requestUrl.Host}/account/confirmemail?userId={userId}&token={encodedToken}";

                var message = $"Please confirm your email by clicking this link: <a href='{confirmationLink}'>Confirm Email</a>";

                await _emailService.SendEmailAsync(user.Email, "Confirm your email", message);
            }

        public async Task DeleteByUserIdAsync(string userId)
        {
            var courier = await _courierRepository.GetByUserIdAsync(userId); // already implemented

            if (courier != null)
            {
                if (courier.User != null)
                {
                    var result = await _userManager.DeleteAsync(courier.User);
                    if (!result.Succeeded)
                    {
                        throw new Exception("Failed to delete associated user: " +
                            string.Join("; ", result.Errors.Select(e => e.Description)));
                    }
                    // Courier will be deleted by cascade if configured
                }
                else
                {
                    await _courierRepository.DeleteAsync(courier); // fallback if user is null
                }
            }
        }



        public async Task<IEnumerable<CourierVM>> GetAllCourierAsync()
        {
            var couriers = await _courierRepository.GetAllAsync(c => c.User);

            return couriers.Select(c => new CourierVM
            {
                Id = c.UserId,
                FullName = c.User?.FullName,
                UserName = c.User?.UserName,
                Email = c.User?.Email,
                VehicleType = c.VehicleType,
                IsAvailable = c.IsAvailable,
                PhoneNumber = c.User.PhoneNumber
            }).ToList();
        }

        public async Task<List<CourierAssignVM>> GetCouriersForAssignmentAsync()
        {
            var couriers = await _courierRepository.GetAllAsync(c => c.User);

            return couriers.Select(c => new CourierAssignVM
            {
                Id = c.Id,
                FullName = c.User.FullName,
                PhoneNumber = c.User.PhoneNumber,
                IsAvailable = c.IsAvailable 
            }).ToList();
        }

        public async Task<CourierDetailVM> GetByUserIdAsync(string userId)
        {
            var courier = await _courierRepository.GetByUserIdAsync(userId);
            if (courier == null) return null;

            return new CourierDetailVM
            {
                Id = courier.Id,
                FullName = courier.User.FullName,
                Email = courier.User.Email,
                PhoneNumber = courier.User.PhoneNumber,
                VehicleType = courier.VehicleType,
                IsAvailable = courier.IsAvailable,
                Orders = courier.Orders?.Select(o => new OrderVM
                {
                    Id = o.Id,
                    CustomerName = o.Customer.User?.FullName,
                    DeliveryAddress = o.DeliveryAddress,
                    Notes = o.Notes,
                    PaymentMethod = o.PaymentMethod,
                    Status = o.Status,
                    TotalPrice = o.TotalPrice,
                    CreatedAt = o.CreatedDate,
                    CourierId = o.CourierId,
                    CourierName = o.Courier.User?.FullName,
                    CourierPhoneNumber = o.Courier.User?.PhoneNumber,
                    CustomerPhoneNumber = o.Customer.User?.PhoneNumber,
                    DeliveredAt = o.DeliveredAt,
                    EstimatedDeliveryTime = o.EstimatedDeliveryTime,

                    RestaurantName = o.OrderItems
                        .Select(oi => oi.MenuItem.Category.Restaurant.Name)
                        .FirstOrDefault() ?? "Unknown",

                    Items = o.OrderItems.Select(oi => new OrderItemVM
                    {
                        MenuItemName = oi.MenuItem.Name,
                        Price = oi.MenuItem.Price,
                        Quantity = oi.Quantity,
                        Image = oi.MenuItem.Image
                    }).ToList()
                }).ToList()
            };
        }

    }
}
