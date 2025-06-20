using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.RestaurantCategory
{
    public class RestaurantCategoryCreateVM
    {
        public string Name { get; set; }
        public IFormFile Image { get; set; }
        public string? ImageName { get; set; }
    }

    public class RestaurantCategoryCreateVMValidator : AbstractValidator<RestaurantCategoryCreateVM>
    {
        public RestaurantCategoryCreateVMValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(20).WithMessage("Name must not exceed 20 characters.");

            RuleFor(x => x.Image)
               .NotNull().WithMessage("Image is required.")
               .Must(file => file != null && file.ContentType.StartsWith("image/"))
               .WithMessage("File must be an image.");
        }
    }
}
