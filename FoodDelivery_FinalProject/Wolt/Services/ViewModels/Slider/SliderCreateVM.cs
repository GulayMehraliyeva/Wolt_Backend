using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.Slider
{
    public class SliderCreateVM
    {
        public IFormFile Image { get; set; }
    }

    public class SliderCreateVMValidator : AbstractValidator<SliderCreateVM>
    {
        public SliderCreateVMValidator()
        {
            RuleFor(x => x.Image)
                .NotNull().WithMessage("Image is required.");               
        }
    }
}
