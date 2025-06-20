using FluentValidation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.MenuCategory
{
    public class MenuCategoryCreateVM
    {
        public string Name { get; set; }
        public int RestaurantId { get; set; }
        public List<SelectListItem>? Restaurants { get; set; }
    }

    public class MenuCategoryCreateVMValidator : AbstractValidator<MenuCategoryCreateVM>
    {
        public MenuCategoryCreateVMValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(20).WithMessage("Name cannot exceed 20 characters.");

            RuleFor(x => x.RestaurantId)
                .GreaterThan(0).WithMessage("Please select a restaurant.");
        }
    }
}
