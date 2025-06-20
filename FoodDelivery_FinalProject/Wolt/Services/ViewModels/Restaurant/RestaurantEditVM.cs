using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.Restaurant
{
    public class RestaurantEditVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public IFormFile? Image { get; set; }
        public string ExistingImage { get; set; }
        public int EstimatedDeliveryTime { get; set; }
        public decimal DeliveryCost { get; set; }
        public int RestaurantCategoryId { get; set; }

        public List<SelectListItem>? Categories { get; set; }
    }

    public class RestaurantEditVMValidator : AbstractValidator<RestaurantEditVM>
    {
        public RestaurantEditVMValidator()
        {
            RuleFor(r => r.Name)
                .NotEmpty()
                .MaximumLength(20);

            RuleFor(r => r.Address)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(r => r.Phone)
                .NotEmpty();

            RuleFor(r => r.EstimatedDeliveryTime)
                .GreaterThan(0);

            RuleFor(r => r.DeliveryCost)
                .GreaterThanOrEqualTo(0);

            RuleFor(r => r.RestaurantCategoryId)
                .GreaterThan(0);

            When(r => r.Image != null, () =>
            {
                RuleFor(r => r.Image)
                    .Must(file => file.Length > 0)
                    .WithMessage("Image file cannot be empty.");
            });
        }
    }

}
