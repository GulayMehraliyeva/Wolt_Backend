using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.Brand
{
    public class BrandEditVM
    {
        public int Id { get; set; }
        public IFormFile UploadImage { get; set; }
        public string CurrentImagePath { get; set; }
    }

    public class BrandEditVMValidator : AbstractValidator<BrandEditVM>
    {
        public BrandEditVMValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid brand ID.");

            RuleFor(x => x.UploadImage)
                .Must(file => file == null || file.ContentType.StartsWith("image/"))
                .WithMessage("Only image files are allowed.");
        }
    }

}
