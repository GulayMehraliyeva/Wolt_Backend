using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Helpers.Exceptions;
using Service.Services.Interfaces;
using Service.ViewModels.Setting;

namespace Wolt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SettingController : Controller
    {
        private readonly ISettingService _settingService;
        private readonly ILogger<SettingController> _logger;

        public SettingController(ISettingService settingService, ILogger<SettingController> logger)
        {
            _settingService = settingService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("SettingController: Index method used");
            var settings = await _settingService.GetAllAsync();
            return View(settings);
        }

        [HttpGet]
        public IActionResult Create()
        {
            _logger.LogInformation("SettingController: Create GET method used");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SettingCreateVM vm)
        {
            _logger.LogInformation("SettingController: Create POST method used");

            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                await _settingService.CreateAsync(vm);
                return RedirectToAction(nameof(Index));
            }
            catch (AppValidationException ex)
            {
                ModelState.AddModelError("Key", ex.Message);
                return View(vm);
            }
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("SettingController: Edit GET method used");

            var setting = await _settingService.GetByIdAsync(id);
            if (setting == null)
            {
                return NotFound();
            }

            return View(setting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SettingEditVM vm)
        {
            _logger.LogInformation("SettingController: Edit POST method used");

            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                await _settingService.UpdateAsync(vm);
                return RedirectToAction(nameof(Index));
            }
            catch (AppValidationException ex)
            {
                if (!ModelState.ContainsKey("Key") ||
                    !ModelState["Key"].Errors.Any(e => e.ErrorMessage == ex.Message))
                {
                    ModelState.AddModelError("Key", ex.Message);
                }
                return View(vm);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("SettingController: Delete method used for Id={Id}", id);
            await _settingService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }

}
