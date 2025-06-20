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
        private readonly ILogger<SliderController> _logger;

        public SliderController(ISliderService sliderService,
                                IWebHostEnvironment environment,
                                ILogger<SliderController> logger)
        {
            _sliderService = sliderService;
            _environment = environment;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("SliderController: Index method used");
            IEnumerable<SliderVM> sliders = await _sliderService.GetAllAsync();
            return View(sliders);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("SliderController: Create GET method used");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderCreateVM request)
        {
            _logger.LogInformation("SliderController: Create POST method used");

            if (request.Image == null)
            {
                _logger.LogWarning("SliderController: Create POST - Image is null");
                ModelState.AddModelError("Image", "Image is required.");
                return View(request);
            }

            if (!request.Image.ContentType.Contains("image/"))
            {
                _logger.LogWarning("SliderController: Create POST - Invalid image content type");
                ModelState.AddModelError("Image", "Only image files are allowed.");
                return View(request);
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("SliderController: Create POST - Model state invalid");
                return View(request);
            }

            await _sliderService.CreateAsync(request);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("SliderController: Delete method used for Id={Id}", id);
            await _sliderService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("SliderController: Edit GET method used for Id={Id}", id);
            var slider = await _sliderService.GetByIdAsync(id);
            if (slider == null)
            {
                _logger.LogWarning("SliderController: Edit GET - Slider not found for Id={Id}", id);
                return NotFound();
            }

            var editVm = new SliderEditVM
            {
                Id = slider.Id,
                CurrentImagePath = slider.Image
            };

            return View(editVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SliderEditVM editVm)
        {
            _logger.LogInformation("SliderController: Edit POST method used for Id={Id}", editVm.Id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("SliderController: Edit POST - Model state invalid for Id={Id}", editVm.Id);
                return View(editVm);
            }

            try
            {
                await _sliderService.EditAsync(editVm.Id, editVm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SliderController: Edit POST - Exception occurred for Id={Id}", editVm.Id);
                ModelState.AddModelError("", ex.Message);
                return View(editVm);
            }

            return RedirectToAction(nameof(Index));
        }
    }

}
