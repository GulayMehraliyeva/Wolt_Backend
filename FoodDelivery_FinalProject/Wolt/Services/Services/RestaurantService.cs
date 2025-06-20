using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Repository.Repositories;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;
using Service.ViewModels.Restaurant;
using Service.ViewModels.RestaurantCategory;
using Service.ViewModels.RestaurantReview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly IRestaurantCategoryRepository _restaurantCategoryRepository;

        public RestaurantService(IRestaurantRepository restaurantRepository,
                                 IMapper mapper,
                                 IWebHostEnvironment environment,
                                 IRestaurantCategoryRepository restaurantCategoryRepository)
        { 
            _environment = environment;
            _mapper = mapper;
            _restaurantRepository = restaurantRepository;
            _restaurantCategoryRepository = restaurantCategoryRepository;
        }
        public async Task CreateAsync(RestaurantCreateVM request)
        {
            var allRestaurants = await _restaurantRepository.GetAllAsync();

            bool nameExists = allRestaurants.Any(r =>
                r.Name.Trim().ToLower() == request.Name.Trim().ToLower());

            if (nameExists)
                throw new Exception("A restaurant with the same name already exists.");

            var restaurant = _mapper.Map<Restaurant>(request);

            string fileName = Guid.NewGuid().ToString() + "-" + request.Image.FileName;
            string folderPath = Path.Combine(_environment.WebRootPath, "assets/images/");
            string filePath = Path.Combine(folderPath, fileName);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                await request.Image.CopyToAsync(stream);
            }

            restaurant.Image = fileName;

            await _restaurantRepository.CreateAsync(restaurant);
        }

        public async Task DeleteAsync(int id)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(id);
            if (restaurant == null) throw new Exception("Restaurant not found");

            if (!string.IsNullOrEmpty(restaurant.Image))
            {
                var imagePath = Path.Combine(_environment.WebRootPath, "assets/images/", restaurant.Image);
                if (File.Exists(imagePath))
                    File.Delete(imagePath);
            }

            await _restaurantRepository.DeleteAsync(restaurant);
        }

        public async Task EditAsync(int id, RestaurantEditVM editVm)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(id);
            if (restaurant == null)
                throw new Exception("Restaurant not found");

            // Check duplicate name excluding the current restaurant
            var allRestaurants = await _restaurantRepository.GetAllAsync();

            bool nameExists = allRestaurants.Any(r =>
                r.Id != id &&
                r.Name.Trim().ToLower() == editVm.Name.Trim().ToLower());

            if (nameExists)
                throw new Exception("A restaurant with the same name already exists.");

            _mapper.Map(editVm, restaurant);

            if (editVm.Image != null)
            {
                if (!string.IsNullOrEmpty(restaurant.Image))
                {
                    var oldImagePath = Path.Combine(_environment.WebRootPath, "assets/images/", restaurant.Image);
                    if (File.Exists(oldImagePath))
                        File.Delete(oldImagePath);
                }

                string fileName = Guid.NewGuid().ToString() + "-" + editVm.Image.FileName;
                string folderPath = Path.Combine(_environment.WebRootPath, "assets/images/");
                string filePath = Path.Combine(folderPath, fileName);

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await editVm.Image.CopyToAsync(stream);
                }

                restaurant.Image = fileName;
            }
            else
            {
                restaurant.Image = editVm.ExistingImage;
            }

            await _restaurantRepository.UpdateAsync(restaurant);
        }

        public async Task<IEnumerable<RestaurantVM>> GetAllAsync()
        {
            var restaurants = await _restaurantRepository.GetAllAsync(
                r => r.RestaurantCategory,
                r => r.Reviews
            );

            var restaurantVMs = _mapper.Map<IEnumerable<RestaurantVM>>(restaurants);

            foreach (var vm in restaurantVMs)
            {
                var restaurant = restaurants.FirstOrDefault(r => r.Id == vm.Id);
                vm.CategoryName = restaurant?.RestaurantCategory?.Name;
                vm.AverageRating = restaurant?.Reviews?.Any() == true
                    ? Math.Round(restaurant.Reviews.Average(r => r.Rating), 1)
                    : 0;
            }

            return restaurantVMs;
        }


        public async Task<RestaurantDetailVM> GetByIdAsync(int id)
        {
            var restaurant = await _restaurantRepository.GetByIdWithIncludesAsync(id, q =>
                q.Include(r => r.RestaurantCategory)
                 .Include(r => r.Reviews)
                     .ThenInclude(rv => rv.Customer)
                         .ThenInclude(c => c.User)
            );

            if (restaurant == null) return null;

            var detailVM = new RestaurantDetailVM
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Address = restaurant.Address,
                Phone = restaurant.Phone,
                Image = restaurant.Image,
                EstimatedDeliveryTime = restaurant.EstimatedDeliveryTime,
                DeliveryCost = restaurant.DeliveryCost,
                CategoryName = restaurant.RestaurantCategory?.Name,
                RestaurantCategoryId = restaurant.RestaurantCategoryId,
                AverageRating = restaurant.Reviews?.Any() == true
                    ? Math.Round(restaurant.Reviews.Average(r => r.Rating), 1)
                    : 0,
                Reviews = restaurant.Reviews?.Select(r => new RestaurantReviewVM
                {
                    CustomerName = r.Customer?.User?.FullName,
                    Rating = r.Rating,
                    Comment = r.Comment
                }).ToList()
            };

            return detailVM;
        }


        public async Task<RestaurantVMUI> GetAllPaginatedWithCategories(int page, int take = 9)
        {
            var restaurants = await _restaurantRepository.GetAllPaginated(page, take);
            var mappedRestaurants = _mapper.Map<List<RestaurantVM>>(restaurants);
            foreach (var vm in mappedRestaurants)
            {
                var restaurant = restaurants.FirstOrDefault(r => r.Id == vm.Id);
                vm.CategoryName = restaurant?.RestaurantCategory?.Name;
                vm.AverageRating = restaurant.Reviews?.Any() == true
                 ? Math.Round(restaurant.Reviews.Average(r => r.Rating), 1)
                 : 0;
            }


            var categories = await _restaurantCategoryRepository.GetAllAsync(r => r.Restaurants); 
            var mappedCategories = _mapper.Map<List<RestaurantCategoryVM>>(categories);

            foreach (var cat in mappedCategories)
            {
                var entity = categories.FirstOrDefault(c => c.Id == cat.Id);
                cat.NumberOfRestaurants = entity?.Restaurants?.Count ?? 0;
            }

            int count = await _restaurantRepository.GetCountAsync();
            int totalPage = (int)Math.Ceiling((decimal)count / take);

            return new RestaurantVMUI
            {
                Restaurants = mappedRestaurants,
                RestaurantCategories = mappedCategories,
                CurrentPage = page,
                TotalPages = totalPage
            };
        }

        public async Task<IEnumerable<RestaurantVM>> SearchRestaurantsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<RestaurantVM>();

            var restaurants = await _restaurantRepository.SearchByNameAsync(query);

            var restaurantVMs = restaurants.Select(r => new RestaurantVM
            {
                Id = r.Id,
                Name = r.Name,
                Address = r.Address,
                Phone = r.Phone,
                Image = r.Image,
                EstimatedDeliveryTime = r.EstimatedDeliveryTime,
                DeliveryCost = r.DeliveryCost,
                RestaurantCategoryId = r.RestaurantCategoryId,
                CategoryName = r.RestaurantCategory?.Name,
            });

            return restaurantVMs;
        }
        public async Task<IEnumerable<RestaurantVM>> GetFilteredAsync(RestaurantFilterVM filter)
        {
            var restaurants = await _restaurantRepository.GetAllAsync(r => r.RestaurantCategory, r => r.Reviews);

            if (filter.FreeDelivery.HasValue && filter.FreeDelivery.Value)
                restaurants = restaurants.Where(r => r.DeliveryCost == 0);

            if (filter.MaxDeliveryTime.HasValue)
                restaurants = restaurants.Where(r => r.EstimatedDeliveryTime <= filter.MaxDeliveryTime.Value);

            if (filter.SelectedRatings != null && filter.SelectedRatings.Any())
            {
                restaurants = restaurants.Where(r =>
                {
                    if (r.Reviews == null || !r.Reviews.Any())
                        return false;

                    double averageRating = r.Reviews.Average(rev => rev.Rating);

                    foreach (int selected in filter.SelectedRatings)
                    {
                        switch (selected)
                        {
                            case 5: 
                                if (averageRating >= 4 && averageRating <= 5)
                                    return true;
                                break;
                            case 4:
                                if (averageRating >= 3 && averageRating < 4)
                                    return true;
                                break;
                            case 3: 
                                if (averageRating >= 2 && averageRating < 3)
                                    return true;
                                break;
                            case 2: 
                                if (averageRating < 2)
                                    return true;
                                break;
                        }
                    }

                    return false;
                });
            }
        
            return restaurants.Select(r => new RestaurantVM
            {
                Id = r.Id,
                Name = r.Name,
                Address = r.Address,
                Phone = r.Phone,
                Image = r.Image,
                EstimatedDeliveryTime = r.EstimatedDeliveryTime,
                DeliveryCost = r.DeliveryCost,
                RestaurantCategoryId = r.RestaurantCategoryId,
                CategoryName = r.RestaurantCategory?.Name,
                AverageRating = r.Reviews?.Any() == true
                    ? Math.Round(r.Reviews.Average(rev => rev.Rating), 1)
                    : 0
            });
        }


        public async Task<IEnumerable<RestaurantVM>> GetByCategoryIdAsync(int categoryId)
        {
            var restaurants = await _restaurantRepository.GetAllAsync(
                r => r.RestaurantCategoryId == categoryId,
                r => r.RestaurantCategory,
                r => r.Reviews);

            return restaurants.Select(r => new RestaurantVM
            {
                Id = r.Id,
                Name = r.Name,
                Address = r.Address,
                Phone = r.Phone,
                Image = r.Image,
                EstimatedDeliveryTime = r.EstimatedDeliveryTime,
                DeliveryCost = r.DeliveryCost,
                RestaurantCategoryId = r.RestaurantCategoryId,
                CategoryName = r.RestaurantCategory?.Name,
                AverageRating = r.Reviews != null && r.Reviews.Any()
                    ? Math.Round(r.Reviews.Average(x => x.Rating), 1)
                    : 0
            });
        }


    }
}
