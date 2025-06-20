using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.RestaurantCategory
{
    public class RestaurantCategoryEditVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IFormFile UploadImage { get; set; }
        public string CurrentImagePath { get; set; }
    }

    public class RestaurantCategoryEditVMValidator : AbstractValidator<RestaurantCategoryEditVM>
    {
        public RestaurantCategoryEditVMValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(20).WithMessage("Name must not exceed 20 characters.");

            When(x => x.UploadImage != null, () =>
            {
                RuleFor(x => x.UploadImage)
                    .Must(file => file.Length > 0).WithMessage("Uploaded file is empty.")
                    .Must(file => new[] { "image/jpeg", "image/png", "image/gif" }
                        .Contains(file.ContentType))
                    .WithMessage("Only JPEG, PNG, or GIF images are allowed.");
            });
        }
    }
}
