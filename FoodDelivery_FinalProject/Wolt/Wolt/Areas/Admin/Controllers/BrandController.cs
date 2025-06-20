using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;
using Service.ViewModels.Brand;

namespace Wolt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BrandController : Controller
    {
        private readonly IBrandService _brandService;
        private readonly ILogger<BrandController> _logger;

        public BrandController(IBrandService brandService, ILogger<BrandController> logger)
        {
            _brandService = brandService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("BrandController: Index method used");
            var brands = await _brandService.GetAllAsync();
            return View(brands);
        }

        [HttpGet]
        public IActionResult Create()
        {
            _logger.LogInformation("BrandController: Create GET method used");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandCreateVM request)
        {
            _logger.LogInformation("BrandController: Create POST method used");

            if (request.Image == null || !request.Image.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Image", "Only image files are allowed.");
                return View(request);
            }

            if (!ModelState.IsValid)
            {
                return View(request);
            }

            await _brandService.CreateAsync(request);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("BrandController: Edit GET method used");

            var brand = await _brandService.GetByIdAsync(id);
            if (brand == null) return NotFound();

            var editVm = new BrandEditVM
            {
                Id = brand.Id,
                CurrentImagePath = brand.Image
            };

            return View(editVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BrandEditVM editVm)
        {
            _logger.LogInformation("BrandController: Edit POST method used");

            if (!ModelState.IsValid)
            {
                return View(editVm);
            }

            try
            {
                await _brandService.EditAsync(editVm.Id, editVm);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(editVm);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("BrandController: Delete method used");

            await _brandService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }

}
