using Microsoft.Extensions.DependencyInjection;
using Service.Services.Interfaces;
using Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServiceLayer(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ILayoutService, LayoutService>();
            services.AddScoped<ISliderService, SliderService>();
            services.AddScoped<IRestaurantCategoryService, RestaurantCategoryService>();
            services.AddScoped<IDiscountService, DiscountService>();
            services.AddScoped<IRestaurantService, RestaurantService>();
            services.AddScoped<IMenuCategoryService, MenuCategoryService>();
            services.AddScoped<IMenuItemService, MenuItemService>();
            services.AddScoped<ICourierService, CourierService>();
            services.AddScoped<IRestaurantReviewService, RestaurantReviewService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IProfileService, ProfileService>();
            services.AddTransient<ISettingService, SettingService>();
            services.AddTransient<IBrandService, BrandService>();

            return services;
        }
    }
}
