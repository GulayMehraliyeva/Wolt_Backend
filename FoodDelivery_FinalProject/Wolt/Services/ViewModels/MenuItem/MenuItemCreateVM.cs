using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.MenuItem
{
    public class MenuItemCreateVM
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public IFormFile Image { get; set; }

        public decimal Price { get; set; }

        public int CategoryId { get; set; }

        public List<SelectListItem>? Categories { get; set; }

        public List<int> SelectedDiscountIds { get; set; } = new();
        public List<SelectListItem>? Discounts { get; set; }

        public int RestaurantId { get; set; }
        public List<SelectListItem>? Restaurants { get; set; }
    }

    public class MenuItemCreateVMValidator : AbstractValidator<MenuItemCreateVM>
    {
        public MenuItemCreateVMValidator()
        {
            RuleFor(x => x.Name)
             .NotEmpty().WithMessage("Name is required.")
             .MaximumLength(20);

            RuleFor(x => x.Description)
                .MaximumLength(30).WithMessage("Description cannot be longer than 30 characters.");

            RuleFor(x => x.Image)
                .NotNull().WithMessage("Image is required.")
                .Must(file => file != null && file.ContentType.StartsWith("image/"))
                .WithMessage("File must be an image.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            RuleFor(x => x.CategoryId)
                .NotEqual(0).WithMessage("You must select a category.");

            RuleFor(x => x.RestaurantId)
                .NotEqual(0).WithMessage("You must select a restaurant.");
        }
    }
}

