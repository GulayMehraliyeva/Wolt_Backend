using AutoMapper;
using Domain.Entities;
using Repository.Repositories;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;
using Service.ViewModels.Discount;
using Service.ViewModels.MenuCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly IMapper _mapper;

        public DiscountService(IDiscountRepository discountRepository,
                               IMapper mapper)
        {
            _discountRepository = discountRepository;
            _mapper = mapper;
        }

        public async Task CreateAsync(DiscountCreateVM request)
        {
            var allDiscounts = await _discountRepository.GetAllAsync();
            bool nameExists = allDiscounts.Any(d =>
                d.Name.Trim().ToLower() == request.Name.Trim().ToLower());

            if (nameExists)
                throw new Exception("A discount with the same name already exists.");

            var discount = _mapper.Map<Discount>(request);
            await _discountRepository.CreateAsync(discount);
        }


        public async Task DeleteAsync(int id)
        {
            var discount = await _discountRepository.GetByIdAsync(id);
            if (discount == null) throw new Exception("Discount not found");
            await _discountRepository.DeleteAsync(discount);
        }

        public async Task EditAsync(int id, DiscountEditVM editVm)
        {
            var discount = await _discountRepository.GetByIdAsync(id);
            if (discount == null)
                throw new Exception("Discount not found");

            var allDiscounts = await _discountRepository.GetAllAsync();
            bool nameExists = allDiscounts.Any(d =>
                d.Id != id && d.Name.Trim().ToLower() == editVm.Name.Trim().ToLower());

            if (nameExists)
                throw new Exception("A discount with the same name already exists.");

            _mapper.Map(editVm, discount);
            await _discountRepository.UpdateAsync(discount);
        }


        public async Task<IEnumerable<DiscountVM>> GetAllAsync()
        {
            await DeactivateExpiredDiscountsAsync();

            var discounts = await _discountRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<DiscountVM>>(discounts);
        }

        public async Task<DiscountVM> GetByIdAsync(int id)
        {
            var discount = await _discountRepository.GetByIdAsync(id);

            if (discount == null) return null;

            return _mapper.Map<DiscountVM>(discount);
        }

        private async Task DeactivateExpiredDiscountsAsync()
        {
            var discounts = await _discountRepository.GetAllAsync();

            var expiredDiscounts = discounts
                .Where(d => d.IsActive && d.EndDate.HasValue && d.EndDate <= DateTime.UtcNow);

            foreach (var discount in expiredDiscounts)
            {
                discount.IsActive = false;
                await _discountRepository.UpdateAsync(discount);
            }
        }
    }
}
