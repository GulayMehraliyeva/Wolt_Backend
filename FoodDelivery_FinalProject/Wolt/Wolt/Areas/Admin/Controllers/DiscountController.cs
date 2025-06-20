using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            {
                _logger.LogWarning("DiscountController: Create POST - Invalid model state");
                return View(request);
            }

            await _discountService.CreateAsync(request);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("DiscountController: Delete method used for ID={Id}", id);
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
                _logger.LogWarning("DiscountController: Edit GET - Discount not found for ID={Id}", id);
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
            _logger.LogInformation("DiscountController: Edit POST method used for ID={Id}", editVm.Id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("DiscountController: Edit POST - Invalid model state for ID={Id}", editVm.Id);
                return View(editVm);
            }

            await _discountService.EditAsync(editVm.Id, editVm);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            _logger.LogInformation("DiscountController: Detail method used for ID={Id}", id);

            var discount = await _discountService.GetByIdAsync(id);
            if (discount == null)
            {
                _logger.LogWarning("DiscountController: Detail - Discount not found for ID={Id}", id);
                return NotFound();
            }

            return View(discount);
        }
    }

}
