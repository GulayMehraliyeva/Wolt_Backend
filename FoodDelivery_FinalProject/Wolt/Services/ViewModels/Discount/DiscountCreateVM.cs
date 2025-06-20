using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.Discount
{
    public class DiscountCreateVM
    {
        public string Name { get; set; }

        public decimal DiscountPercentage { get; set; }

        public bool IsActive { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
    }

    public class DiscountCreateVMValidator : AbstractValidator<DiscountCreateVM>
    {
        public DiscountCreateVMValidator()
        {
            RuleFor(d => d.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(20).WithMessage("Name cannot exceed 20 characters.");

            RuleFor(d => d.DiscountPercentage)
                .InclusiveBetween(0, 100).WithMessage("Discount percentage must be between 0 and 100.");

            RuleFor(d => d.IsActive)
                .NotNull().WithMessage("IsActive must be specified.");

            RuleFor(d => d.EndDate)
                 .Must(date => date.Date >= DateTime.Today)
                 .WithMessage("End date cannot be in the past.");
        }
    }
}
