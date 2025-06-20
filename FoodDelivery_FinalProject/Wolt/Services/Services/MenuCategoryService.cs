using AutoMapper;
using Domain.Entities;
using Repository.Repositories;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;
using Service.ViewModels.MenuCategory;
using Service.ViewModels.Restaurant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class MenuCategoryService : IMenuCategoryService
    {
        private readonly IMenuCategoryRepository _menuCategoryRepository;
        private readonly IMapper _mapper;

        public MenuCategoryService(IMenuCategoryRepository menuCategoryRepository,
                                   IMapper mapper)
        {
            _menuCategoryRepository = menuCategoryRepository;
            _mapper = mapper;
        }

        public async Task CreateAsync(MenuCategoryCreateVM request)
        {
            var menuCategory = _mapper.Map<MenuItemCategory>(request);
            await _menuCategoryRepository.CreateAsync(menuCategory);
        }

        public async Task DeleteAsync(int id)
        {
            var menuCategory = await _menuCategoryRepository.GetByIdAsync(id);
            if (menuCategory == null) throw new Exception("Category not found");
            await _menuCategoryRepository.DeleteAsync(menuCategory);
        }

        public async Task EditAsync(int id, MenuCategoryEditVM editVm)
        {
            var menuCategory = await _menuCategoryRepository.GetByIdAsync(id);
            if (menuCategory == null) throw new Exception("Category not found");

            _mapper.Map(editVm, menuCategory);
            await _menuCategoryRepository.UpdateAsync(menuCategory);
        }

        public async Task<IEnumerable<MenuCategoryVM>> GetAllAsync()
        {
            var menuCategories = await _menuCategoryRepository.GetAllAsync(r => r.Restaurant);

            var menuCategoryVm = _mapper.Map<IEnumerable<MenuCategoryVM>>(menuCategories);

            foreach (var vm in menuCategoryVm)
            {
                var category = menuCategories.FirstOrDefault(r => r.Id == vm.Id);
                vm.RestaurantName = category?.Restaurant?.Name;
            }

            return menuCategoryVm;
        }

        public async Task<MenuCategoryVM> GetByIdAsync(int id)
        {
            var menuCategory = await _menuCategoryRepository.GetByIdAsync(id, r => r.Restaurant);

            if (menuCategory == null) return null;

            var menuCategoryVm = _mapper.Map<MenuCategoryVM>(menuCategory);
            menuCategoryVm.RestaurantName = menuCategory.Restaurant?.Name;

            return menuCategoryVm;
        }

        public async Task<IEnumerable<MenuCategoryVM>> GetByRestaurantIdAsync(int restaurantId)
        {
            var allCategories = await _menuCategoryRepository.GetAllAsync(r => r.Restaurant);

            var filteredCategories = allCategories
                .Where(mc => mc.RestaurantId == restaurantId)
                .ToList();

            var menuCategoryVms = _mapper.Map<IEnumerable<MenuCategoryVM>>(filteredCategories);

            foreach (var vm in menuCategoryVms)
            {
                var category = filteredCategories.FirstOrDefault(r => r.Id == vm.Id);
                vm.RestaurantName = category?.Restaurant?.Name;
            }

            return menuCategoryVms;
        }


    }
}
