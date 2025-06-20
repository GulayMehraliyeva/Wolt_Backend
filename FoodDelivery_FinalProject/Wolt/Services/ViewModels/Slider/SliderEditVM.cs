using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.Slider
{
    public class SliderEditVM
    {
        public int Id { get; set; }
        public IFormFile UploadImage { get; set; }
        public string CurrentImagePath { get; set; }
    }

    public class SliderEditVMValidator : AbstractValidator<SliderEditVM>
    {
        public SliderEditVMValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid slider ID.");

            RuleFor(x => x.UploadImage)
                .Must(file => file == null || file.ContentType.StartsWith("image/"))
                    .WithMessage("Only image files are allowed.");
        }
    }
}
