using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;
using Service.ViewModels.Customer;
using Service.ViewModels.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly UserManager<AppUser> _userManager;


        public CustomerService(ICustomerRepository customerRepository,
                               UserManager<AppUser> userManager)
        {
            _customerRepository = customerRepository;
            _userManager = userManager;
        }

        public Task<Customer> GetByUserIdAsync(string userId)
        {
            return _customerRepository.GetByUserIdAsync(userId);
        }
        public async Task<List<CustomerVM>> GetAllCustomerVMsAsync()
        {
            var customers = await _customerRepository.GetAllWithUsersAsync();

            var customerVMs = customers.Select(c => new CustomerVM
            {
                Id = c.Id,
                FullName = c.User.FullName,
                UserName = c.User.UserName,
                Email = c.User.Email,
                PhoneNumber = c.User.PhoneNumber,
                Address = c.Address,
                UserId = c.UserId,
            }).ToList();

            return customerVMs;
        }

        public async Task<CustomerDetailVM> GetDetailAsync(int id)
        {
            var customer = await _customerRepository.GetByIdWithAllDetailsAsync(id);
            if (customer == null) return null;

            var detailVm = new CustomerDetailVM
            {
                Id = customer.Id,
                FullName = customer.User.FullName,
                Email = customer.User.Email,
                UserName = customer.User.UserName,
                PhoneNumber = customer.User.PhoneNumber,
                Address = customer.Address,
                Orders = customer.Orders?.Select(order => new OrderVM
                {
                    Id = order.Id,
                    CustomerName = customer.User.FullName,
                    DeliveryAddress = order.DeliveryAddress,
                    Notes = order.Notes,
                    PaymentMethod = order.PaymentMethod,
                    Status = order.Status,
                    TotalPrice = order.TotalPrice,
                    CreatedAt = order.CreatedDate,
                    EstimatedDeliveryTime = order.EstimatedDeliveryTime,
                    DeliveredAt = order.DeliveredAt,
                    CourierId = order.CourierId,
                    CourierName = order.Courier?.User.FullName,
                    CourierPhoneNumber = order.Courier?.User.PhoneNumber,
                    CustomerPhoneNumber = customer.User.PhoneNumber,

                    RestaurantName = order.OrderItems != null && order.OrderItems.Any()
        ? order.OrderItems.First().MenuItem?.Category?.Restaurant?.Name ?? "Unknown"
        : "Unknown",

                    Items = order.OrderItems?.Select(item => new OrderItemVM
                    {
                        MenuItemName = item.MenuItem?.Name,
                        Quantity = item.Quantity,
                        Price = item.MenuItem?.Price ?? 0,
                    }).ToList() ?? new List<OrderItemVM>()

                }).ToList()
            };

            return detailVm;
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await _customerRepository.GetByIdWithAllDetailsAsync(id); 
            if (customer != null)
            {
                if (customer.User != null)
                {
                    var result = await _userManager.DeleteAsync(customer.User);
                    if (!result.Succeeded)
                    {
                        throw new Exception("Failed to delete associated user: " +
                                            string.Join("; ", result.Errors.Select(e => e.Description)));
                    }
                }

            }
        }

    }
}
