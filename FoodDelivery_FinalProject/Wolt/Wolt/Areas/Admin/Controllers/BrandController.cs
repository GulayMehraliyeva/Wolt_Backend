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

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var brands = await _brandService.GetAllAsync();
            return View(brands);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandCreateVM request)
        {
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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var brand = await _brandService.GetByIdAsync(id);
            if (brand == null) return NotFound();

            var editVm = new BrandEditVM
            {
                Id = brand.Id,
                CurrentImagePath = brand.Image
            };

            return View(editVm);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BrandEditVM editVm)
        {
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
            await _brandService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }

}
