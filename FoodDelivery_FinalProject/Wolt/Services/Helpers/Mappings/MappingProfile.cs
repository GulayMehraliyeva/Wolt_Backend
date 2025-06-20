using AutoMapper;
using Domain.Entities;
using Service.ViewModels.Brand;
using Service.ViewModels.Cart;
using Service.ViewModels.Discount;
using Service.ViewModels.MenuCategory;
using Service.ViewModels.MenuItem;
using Service.ViewModels.Order;
using Service.ViewModels.Restaurant;
using Service.ViewModels.RestaurantCategory;
using Service.ViewModels.RestaurantReview;
using Service.ViewModels.Slider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Slider, SliderVM>();
            CreateMap<SliderCreateVM, Slider>();
            CreateMap<SliderEditVM, Slider>()
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.CurrentImagePath));

            CreateMap<Brand, BrandVM>();
            CreateMap<BrandCreateVM, Brand>();
            CreateMap<BrandEditVM, Brand>()
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.CurrentImagePath));

            CreateMap<RestaurantCategory, RestaurantCategoryVM>();
            CreateMap<RestaurantCategoryCreateVM, RestaurantCategory>();
            CreateMap<RestaurantCategoryEditVM, RestaurantCategory>()
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.CurrentImagePath));


            CreateMap<Restaurant, RestaurantVM>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.RestaurantCategory.Name));
            CreateMap<RestaurantCreateVM, Restaurant>()
                .ForMember(dest => dest.RestaurantCategoryId, opt => opt.MapFrom(src => src.CategoryId));
            CreateMap<RestaurantEditVM, Restaurant>()
                .ForMember(dest => dest.RestaurantCategoryId, opt => opt.MapFrom(src => src.RestaurantCategoryId));


            CreateMap<MenuItemCategory, MenuCategoryVM>();
            CreateMap<MenuCategoryCreateVM, MenuItemCategory>();
            CreateMap<MenuCategoryEditVM, MenuItemCategory>();

            CreateMap<Discount, DiscountVM>();
            CreateMap<DiscountCreateVM, Discount>();
            CreateMap<DiscountEditVM, Discount>();

            CreateMap<MenuItem, MenuItemVM>()
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image));
            CreateMap<MenuItemCreateVM, MenuItem>()
                .ForMember(dest => dest.MenuItemDiscounts, opt => opt.Ignore())
                .ForMember(dest => dest.Image, opt => opt.Ignore());
            CreateMap<MenuItemEditVM, MenuItem>();

            CreateMap<RestaurantReview, RestaurantReviewVM>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.User.FullName));
            CreateMap<RestaurantReviewCreateVM, RestaurantReview>();
            CreateMap<RestaurantReview, RestaurantReviewCreateVM>();


            CreateMap<CartItem, CartItemVM>()
                .ForMember(dest => dest.MenuItemName, opt => opt.MapFrom(src => src.MenuItem.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.MenuItem.Price))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.MenuItem.Image))
                .ForMember(dest => dest.RestaurantName, opt => opt.MapFrom(src => src.MenuItem.Category.Restaurant.Name));

            CreateMap<Order, OrderVM>()
           .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.User.FullName))
           .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems))
           .ForMember(dest => dest.CourierPhoneNumber, opt => opt.MapFrom(src => src.Courier.User.PhoneNumber));

            CreateMap<OrderItem, OrderItemVM>()
                .ForMember(dest => dest.MenuItemName, opt => opt.MapFrom(src => src.MenuItem.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.MenuItem.Price));
        }
    }
}
