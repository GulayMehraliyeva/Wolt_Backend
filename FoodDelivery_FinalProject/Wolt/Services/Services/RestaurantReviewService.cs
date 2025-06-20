using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Repository.Repositories.Interfaces;
using Service.Helpers.Enums;
using Service.Services.Interfaces;
using Service.ViewModels.Discount;
using Service.ViewModels.RestaurantReview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class RestaurantReviewService : IRestaurantReviewService
    {
        private readonly IRestaurantReviewRepository _restaurantReviewRepository;
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICustomerRepository _customerRepository;

        public RestaurantReviewService(IRestaurantReviewRepository restaurantReviewRepository,
                                       IAccountService accountService,
                                       IMapper mapper,
                                       UserManager<AppUser> userManager,
                                       ICustomerRepository customerRepository)
        {
            _restaurantReviewRepository = restaurantReviewRepository;
            _accountService = accountService;
            _mapper = mapper;
            _userManager = userManager;
            _customerRepository = customerRepository;
        }
        public async Task CreateAsync(RestaurantReviewCreateVM request)
        {
            var user = await _accountService.GetCurrentUserAsync();

            if (user == null || !await _userManager.IsInRoleAsync(user, "Customer"))
                throw new UnauthorizedAccessException("Only customers can submit reviews.");

            var customer = await _customerRepository.GetByUserIdAsync(user.Id);
            if (customer == null)
                throw new Exception("Customer profile not found.");

            var existingReviews = await _restaurantReviewRepository.GetAllByRestaurantIdAsync(request.RestaurantId);
            var alreadyReviewed = existingReviews.Any(r => r.CustomerId == customer.Id);

            if (alreadyReviewed)
                throw new InvalidOperationException("You have already submitted a review for this restaurant.");

            var entity = _mapper.Map<RestaurantReview>(request);
            entity.CustomerId = customer.Id;

            await _restaurantReviewRepository.CreateAsync(entity);
        }


        public async Task DeleteAsync(int id)
        {
            var user = await _accountService.GetCurrentUserAsync();

            if (user == null || (!await _userManager.IsInRoleAsync(user, "Customer") && !await _userManager.IsInRoleAsync(user, "Admin")))
                throw new UnauthorizedAccessException("Only customers and admins can delete reviews.");

            var review = await _restaurantReviewRepository.GetByIdAsync(id);
            if (review == null) throw new Exception("Review not found.");

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await _restaurantReviewRepository.DeleteAsync(review);
                return;
            }

            var customer = await _customerRepository.GetByUserIdAsync(user.Id);
            if (customer == null) throw new Exception("Customer not found.");

            if (review.CustomerId == customer.Id)
            {
                await _restaurantReviewRepository.DeleteAsync(review);
            }
            else
            {
                throw new UnauthorizedAccessException("You can only delete your own review. Admins can delete any review.");
            }
        }

        public async Task<IEnumerable<RestaurantReviewVM>> GetAllAsync()
        {
            var reviews = await _restaurantReviewRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<RestaurantReviewVM>>(reviews);
        }

        public async Task<RestaurantReviewCreateVM> GetByIdAsync(int id)
        {
            var review = await _restaurantReviewRepository.GetByIdAsync(id);
            if (review == null) throw new Exception("Review not found");

            return _mapper.Map<RestaurantReviewCreateVM>(review);
        }

        public async Task<IEnumerable<RestaurantReviewVM>> GetAllByRestaurantIdAsync(int restaurantId)
        {
            var reviews = await _restaurantReviewRepository.GetAllByRestaurantIdAsync(restaurantId);
            return _mapper.Map<IEnumerable<RestaurantReviewVM>>(reviews);
        }
    }
}
