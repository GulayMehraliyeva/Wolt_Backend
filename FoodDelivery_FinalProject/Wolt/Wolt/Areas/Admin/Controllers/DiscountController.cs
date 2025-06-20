using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Helpers.Exceptions;
using Service.Services;
using Service.Services.Interfaces;
using Service.ViewModels.Discount;

namespace Wolt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DiscountController : Controller
    {
        private readonly IDiscountService _discountService;
        private readonly ILogger<DiscountController> _logger;

        public DiscountController(IDiscountService discountService, ILogger<DiscountController> logger)
        {
            _discountService = discountService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("DiscountController: Index method used");
            IEnumerable<DiscountVM> discounts = await _discountService.GetAllAsync();
            return View(discounts);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("DiscountController: Create GET method used");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DiscountCreateVM request)
        {
            _logger.LogInformation("DiscountController: Create POST method used");
            if (!ModelState.IsValid)
                return View(request);

            try
            {
                await _discountService.CreateAsync(request);
                return RedirectToAction(nameof(Index));
            }
            catch (AppValidationException ex)
            {
                ModelState.AddModelError("Name", ex.Message);
                return View(request);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("DiscountController: Delete method used");
            await _discountService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("DiscountController: Edit GET method used for ID={Id}", id);

            var discount = await _discountService.GetByIdAsync(id);
            if (discount == null)
            {
                return NotFound();
            }

            var editVm = new DiscountEditVM
            {
                Id = discount.Id,
                Name = discount.Name,
                DiscountPercentage = discount.DiscountPercentage,
                IsActive = discount.IsActive,
                EndDate = discount.EndDate
            };

            return View(editVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DiscountEditVM editVm)
        {
            _logger.LogInformation("DiscountController: Edit POST method used");

            if (!ModelState.IsValid)
                return View(editVm);

            try
            {
                await _discountService.EditAsync(editVm.Id, editVm);
                return RedirectToAction(nameof(Index));
            }
            catch (AppValidationException ex)
            {
                if (!ModelState.ContainsKey("Name") ||
                    !ModelState["Name"].Errors.Any(e => e.ErrorMessage == ex.Message))
                {
                    ModelState.AddModelError("Name", ex.Message);
                }

                return View(editVm);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            _logger.LogInformation("DiscountController: Detail method used for ID={Id}", id);

            var discount = await _discountService.GetByIdAsync(id);
            if (discount == null)
            {
                return NotFound();
            }

            return View(discount);
        }
    }

}
