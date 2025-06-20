using AutoMapper;
using Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository.Repositories;
using Repository.Repositories.Interfaces;
using Service.Helpers;
using Service.Services.Interfaces;
using Service.ViewModels.MenuItem;
using Service.ViewModels.Restaurant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly IValidator<MenuItemCreateVM> _createValidator;
        private readonly IValidator<MenuItemEditVM> _editValidator;

        public MenuItemService(IMenuItemRepository menuItemRepository,
                               IMapper mapper,
                               IWebHostEnvironment environment,
                               IValidator<MenuItemCreateVM> createValidator,
                               IValidator<MenuItemEditVM> editValidator)
        {
            _menuItemRepository = menuItemRepository;
            _mapper = mapper;
            _environment = environment;
            _createValidator = createValidator;
            _editValidator = editValidator;
        }

        public async Task CreateAsync(MenuItemCreateVM request)
        {
            var validationResult = await _createValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join("\n", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException(errorMessages);
            }

            var allItems = await _menuItemRepository.GetQueryable()
                .Include(m => m.Category)
                .ToListAsync();

            bool nameExists = allItems.Any(m =>
                m.Category.RestaurantId == request.RestaurantId &&  // You need RestaurantId in VM or get it via category
                m.Name.Trim().ToLower() == request.Name.Trim().ToLower());

            if (nameExists)
                throw new Exception("A menu item with the same name already exists in this restaurant.");

            var menuItem = _mapper.Map<MenuItem>(request);

            string fileName = Guid.NewGuid().ToString() + "-" + request.Image.FileName;
            string folderPath = Path.Combine(_environment.WebRootPath, "assets/images/");
            string filePath = Path.Combine(folderPath, fileName);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                await request.Image.CopyToAsync(stream);
            }
            menuItem.Image = fileName;

            if (request.SelectedDiscountIds != null && request.SelectedDiscountIds.Any())
            {
                menuItem.MenuItemDiscounts = request.SelectedDiscountIds
                    .Select(discountId => new MenuItemDiscounts { DiscountId = discountId })
                    .ToList();
            }

            await _menuItemRepository.CreateAsync(menuItem);
        }


        public async Task DeleteAsync(int id)
        {
            var menuItem = await _menuItemRepository.GetByIdAsync(id);
            if (menuItem == null) throw new Exception("MenuItem not found");

            if (!string.IsNullOrEmpty(menuItem.Image))
            {
                var imagePath = Path.Combine(_environment.WebRootPath, "assets/images/", menuItem.Image);
                if (File.Exists(imagePath))
                    File.Delete(imagePath);
            }

            await _menuItemRepository.DeleteAsync(menuItem);
        }

        public async Task EditAsync(MenuItemEditVM model)
        {
            var validationResult = await _editValidator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join("\n", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException(errorMessages);
            }

            var menuItem = await _menuItemRepository.GetQueryable()
                .Include(m => m.Category)
                .Include(m => m.MenuItemDiscounts)
                .FirstOrDefaultAsync(m => m.Id == model.Id);

            if (menuItem == null)
                throw new Exception("MenuItem not found");

            var allItems = await _menuItemRepository.GetQueryable()
                .Include(m => m.Category)
                .ToListAsync();

            bool nameExists = allItems.Any(m =>
                m.Id != model.Id &&
                m.Category.RestaurantId == menuItem.Category.RestaurantId &&
                m.Name.Trim().ToLower() == model.Name.Trim().ToLower());

            if (nameExists)
                throw new Exception("A menu item with the same name already exists in this restaurant.");

            menuItem.Name = model.Name;
            menuItem.Description = model.Description;
            menuItem.Price = model.Price;
            menuItem.CategoryId = model.CategoryId;

            if (model.Image != null)
            {
                string folderPath = Path.Combine(_environment.WebRootPath, "assets/images/");

                if (!string.IsNullOrEmpty(menuItem.Image))
                {
                    string oldImagePath = Path.Combine(folderPath, menuItem.Image);
                    if (File.Exists(oldImagePath))
                        File.Delete(oldImagePath);
                }

                string fileName = Guid.NewGuid().ToString() + "-" + model.Image.FileName;
                string filePath = Path.Combine(folderPath, fileName);

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }

                menuItem.Image = fileName;
            }

            menuItem.MenuItemDiscounts.Clear();
            if (model.DiscountIds != null && model.DiscountIds.Any())
            {
                menuItem.MenuItemDiscounts = model.DiscountIds
                    .Select(id => new MenuItemDiscounts
                    {
                        MenuItemId = model.Id,
                        DiscountId = id
                    }).ToList();
            }

            await _menuItemRepository.UpdateAsync(menuItem);
        }

        public async Task<IEnumerable<MenuItemVM>> GetAllAsync()
        {
            var menuItems = await _menuItemRepository.GetQueryable()
                .Include(r => r.Category)
                .Include(r => r.MenuItemDiscounts)
                    .ThenInclude(md => md.Discount)
                .ToListAsync();

            var menuItemVMs = _mapper.Map<IEnumerable<MenuItemVM>>(menuItems);

            foreach (var vm in menuItemVMs)
            {
                var entity = menuItems.FirstOrDefault(r => r.Id == vm.Id);
                vm.Image = entity?.Image;
                vm.CategoryName = entity?.Category?.Name;
                vm.CategoryId = entity?.CategoryId ?? 0;
                vm.RestaurantId = entity?.Category?.RestaurantId ?? 0;

                decimal originalPrice = entity.Price;

                var activeDiscountPercentages = entity.MenuItemDiscounts?
                    .Where(md => md.Discount?.IsActive == true &&
                                 (md.Discount.EndDate == null || md.Discount.EndDate > DateTime.Now))
                    .Select(md => md.Discount.DiscountPercentage)
                    .ToList() ?? new List<decimal>();

                vm.DiscountedPrice = DiscountCalculator.ApplyDiscounts(originalPrice, activeDiscountPercentages);
            }

            return menuItemVMs;
        }


        public async Task<MenuItemVM> GetByIdAsync(int id)
        {
            var menuItem = await _menuItemRepository.GetQueryable()
                .Include(r => r.Category)
                .Include(r => r.MenuItemDiscounts)
                    .ThenInclude(md => md.Discount)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (menuItem == null)
                return null;

            var vm = _mapper.Map<MenuItemVM>(menuItem);

            vm.CategoryName = menuItem.Category?.Name;
            vm.CategoryId = menuItem.CategoryId;
            vm.RestaurantId = menuItem.Category?.RestaurantId ?? 0;

            decimal originalPrice = menuItem.Price;

            var activeDiscountPercentages = menuItem.MenuItemDiscounts?
                .Where(md => md.Discount?.IsActive == true &&
                             (md.Discount.EndDate == null || md.Discount.EndDate > DateTime.Now))
                .Select(md => md.Discount.DiscountPercentage)
                .ToList() ?? new List<decimal>();

            vm.DiscountedPrice = DiscountCalculator.ApplyDiscounts(originalPrice, activeDiscountPercentages);

            return vm;
        }


        public async Task<IEnumerable<MenuItemVM>> GetByCategoryIdAsync(int categoryId)
        {
            var items = await _menuItemRepository.GetAllAsync(
                m => m.CategoryId == categoryId,
                m => m.Category
            );

            return _mapper.Map<IEnumerable<MenuItemVM>>(items);
        }

        public async Task<MenuItemDetailVM> DetailAsync(int id)
        {
            var menuItem = await _menuItemRepository.GetQueryable()
                .Include(m => m.Category)
                    .ThenInclude(c => c.Restaurant)
                .Include(m => m.MenuItemDiscounts)
                    .ThenInclude(md => md.Discount)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (menuItem == null)
                throw new Exception("Menu item not found");

            var detailVM = new MenuItemDetailVM
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Image = menuItem.Image,
                Price = menuItem.Price,
                CategoryName = menuItem.Category?.Name,
                RestaurantName = menuItem.Category?.Restaurant?.Name,
                DiscountNames = menuItem.MenuItemDiscounts
                    .Where(md => md.Discount != null && md.Discount.IsActive &&
                                 (md.Discount.EndDate == null || md.Discount.EndDate > DateTime.Now))
                    .Select(md => md.Discount.Name)
                    .ToList()
            };

            return detailVM;
        }

        public async Task<IEnumerable<MenuItemVM>> GetByRestaurantIdAsync(int restaurantId)
        {
            var menuItems = await _menuItemRepository.GetQueryable()
                .Include(m => m.Category)
                .Include(m => m.MenuItemDiscounts)
                    .ThenInclude(md => md.Discount)
                .Where(m => m.Category.RestaurantId == restaurantId)
                .ToListAsync();

            var menuItemVMs = _mapper.Map<IEnumerable<MenuItemVM>>(menuItems);

            foreach (var vm in menuItemVMs)
            {
                var entity = menuItems.FirstOrDefault(r => r.Id == vm.Id);
                vm.Image = entity?.Image;
                vm.CategoryName = entity?.Category?.Name;
                vm.CategoryId = entity?.CategoryId ?? 0;
                vm.RestaurantId = entity?.Category?.RestaurantId ?? 0;

                decimal originalPrice = entity.Price;

                var activeDiscountPercentages = entity.MenuItemDiscounts?
                    .Where(md => md.Discount?.IsActive == true &&
                                 (md.Discount.EndDate == null || md.Discount.EndDate > DateTime.Now))
                    .Select(md => md.Discount.DiscountPercentage)
                    .ToList() ?? new List<decimal>();

                vm.DiscountedPrice = DiscountCalculator.ApplyDiscounts(originalPrice, activeDiscountPercentages);
            }

            return menuItemVMs;
        }


    }
}
