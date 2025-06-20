using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.Discount
{
    public class DiscountEditVM
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(0, 100)]
        public decimal DiscountPercentage { get; set; }

        public bool IsActive { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
    }

    public class DiscountEditVMValidator : AbstractValidator<DiscountEditVM>
    {
        public DiscountEditVMValidator()
        {
            RuleFor(d => d.Id)
                .GreaterThan(0).WithMessage("Id must be greater than zero.");

            RuleFor(d => d.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(d => d.DiscountPercentage)
                .InclusiveBetween(0, 100).WithMessage("Discount percentage must be between 0 and 100.");

            RuleFor(d => d.IsActive)
                .NotNull().WithMessage("IsActive must be specified.");

            RuleFor(d => d.EndDate)
                .Must(date => date == null || date.Value.Date >= DateTime.Today)
                .WithMessage("EndDate cannot be in the past.");
        }
    }
}
