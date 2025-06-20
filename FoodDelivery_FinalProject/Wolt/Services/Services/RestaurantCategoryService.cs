using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Repository.Repositories.Interfaces;
using Service.Helpers.Exceptions;
using Service.Services.Interfaces;
using Service.ViewModels.RestaurantCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class RestaurantCategoryService : IRestaurantCategoryService
    {
        private readonly IRestaurantCategoryRepository _restaurantCategoryRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly IRestaurantRepository _restaurantRepository;

        public RestaurantCategoryService(IRestaurantCategoryRepository restaurantCategoryRepository, 
                                         IMapper mapper,
                                         IWebHostEnvironment environment,
                                         IRestaurantRepository restaurantRepository)
        {
            _restaurantCategoryRepository = restaurantCategoryRepository;
            _mapper = mapper;
            _environment = environment;
            _restaurantRepository = restaurantRepository;
        }

        public async Task CreateAsync(RestaurantCategoryCreateVM request)
        {
            var allCategories = await _restaurantCategoryRepository.GetAllAsync();

            bool nameExists = allCategories.Any(c =>
                c.Name.Trim().ToLower() == request.Name.Trim().ToLower());

            if (nameExists)
                throw new AppValidationException("A restaurant category with the same name already exists.");

            var restaurantCategory = _mapper.Map<RestaurantCategory>(request);

            string fileName = Guid.NewGuid().ToString() + "-" + request.Image.FileName;
            string folderPath = Path.Combine(_environment.WebRootPath, "assets/images/");
            string filePath = Path.Combine(folderPath, fileName);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                await request.Image.CopyToAsync(stream);
            }

            restaurantCategory.Image = fileName;
            await _restaurantCategoryRepository.CreateAsync(restaurantCategory);
        }

        public async Task DeleteAsync(int id)
        {
            var restaurantCategory = await _restaurantCategoryRepository.GetByIdAsync(id);
            if (restaurantCategory == null)
                throw new Exception("Category not found");

            string imagePath = Path.Combine(_environment.WebRootPath, "assets/images", restaurantCategory.Image);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            await _restaurantCategoryRepository.DeleteAsync(restaurantCategory);
        }

        public async Task EditAsync(int id, RestaurantCategoryEditVM editVm)
        {
            var existing = await _restaurantCategoryRepository.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Category not found");

            var allCategories = await _restaurantCategoryRepository.GetAllAsync();

            bool nameExists = allCategories.Any(c =>
                c.Id != id &&
                c.Name.Trim().ToLower() == editVm.Name.Trim().ToLower());

            if (nameExists)
                throw new AppValidationException("A restaurant category with the same name already exists.");


            _mapper.Map(editVm, existing);

            if (editVm.UploadImage != null)
            {
                if (!editVm.UploadImage.ContentType.Contains("image/"))
                    throw new Exception("Input type must be only image");

                string oldImagePath = Path.Combine(_environment.WebRootPath, "assets/images", existing.Image);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }

                string fileName = Guid.NewGuid().ToString() + "-" + editVm.UploadImage.FileName;
                string folderPath = Path.Combine(_environment.WebRootPath, "assets/images");
                string newImagePath = Path.Combine(folderPath, fileName);

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                using (FileStream stream = new FileStream(newImagePath, FileMode.Create))
                {
                    await editVm.UploadImage.CopyToAsync(stream);
                }

                existing.Image = fileName;
            }

            await _restaurantCategoryRepository.UpdateAsync(existing);
        }

        public async Task<IEnumerable<RestaurantCategoryVM>> GetAllAsync()
        {
            var categories = await _restaurantCategoryRepository.GetAllAsync(c => true, c => c.Restaurants);
            var categoryVMs = _mapper.Map<List<RestaurantCategoryVM>>(categories);

            foreach (var categoryVm in categoryVMs)
            {
                var categoryEntity = categories.First(c => c.Id == categoryVm.Id);
                categoryVm.NumberOfRestaurants = categoryEntity.Restaurants?.Count ?? 0;
            }

            return categoryVMs;
        }



        public async Task<RestaurantCategoryVM> GetByIdAsync(int id)
        {
            var category = await _restaurantCategoryRepository.GetByIdAsync(id, c => c.Restaurants);
            if (category == null) return null;

            var categoryVm = _mapper.Map<RestaurantCategoryVM>(category);

            categoryVm.NumberOfRestaurants = category.Restaurants?.Count ?? 0;

            return categoryVm;
        }

    }
}
