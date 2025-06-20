using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;
using System.Reflection;
using Service.ViewModels.Slider;
using Microsoft.AspNetCore.Authorization;

namespace Wolt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SliderController : Controller
    {
        private readonly ISliderService _sliderService;
        private readonly IWebHostEnvironment _environment;

        public SliderController(ISliderService sliderService,
                                IWebHostEnvironment environment)
        {
            _sliderService = sliderService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<SliderVM> sliders =await _sliderService.GetAllAsync();
            return View(sliders);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderCreateVM request)
        {
            if (request.Image == null)
            {
                ModelState.AddModelError("Image", "Image is required.");
                return View(request);
            }

            if (!request.Image.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Image", "Only image files are allowed.");
                return View(request);
            }

            if (!ModelState.IsValid) return View(request);

            await _sliderService.CreateAsync(request);
            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _sliderService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var slider = await _sliderService.GetByIdAsync(id);
            if (slider == null) return NotFound();

            var editVm = new SliderEditVM
            {
                Id = slider.Id,
                CurrentImagePath = slider.Image
            };

            return View(editVm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SliderEditVM editVm)
        {
            if (!ModelState.IsValid)
            {
                return View(editVm);
            }

            try
            {
                await _sliderService.EditAsync(editVm.Id, editVm);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(editVm);
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
