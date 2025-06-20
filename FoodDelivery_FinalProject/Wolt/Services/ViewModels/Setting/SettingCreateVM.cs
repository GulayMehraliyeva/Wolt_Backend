using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.Setting
{
    public class SettingCreateVM
    {
        public string Key { get; set; }

        public string? Value { get; set; } 

        public IFormFile? Image { get; set; }

        public string Type { get; set; }
    }

    public class SettingCreateVMValidator : AbstractValidator<SettingCreateVM>
    {
        public SettingCreateVMValidator()
        {
            RuleFor(x => x.Key)
                .NotEmpty().WithMessage("Key is required.")
                .MaximumLength(30).WithMessage("Key must be at most 30 characters.");

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Type is required.")
                .MaximumLength(30).WithMessage("Type must be at most 30 characters.");

            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.Value) || x.Image != null)
                .WithMessage("Either Value or Image must be provided.");
        }
    }
}
