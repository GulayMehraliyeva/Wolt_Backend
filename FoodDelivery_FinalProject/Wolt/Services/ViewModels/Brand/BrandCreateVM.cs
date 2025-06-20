using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.Brand
{
    public class BrandCreateVM
    {
        public IFormFile Image { get; set; }
    }

    public class BrandCreateVMValidator : AbstractValidator<BrandCreateVM>
    {
        public BrandCreateVMValidator()
        {
            RuleFor(x => x.Image)
                .NotNull().WithMessage("Image is required.");
        }
    }
}
