using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;

namespace Wolt.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly ILayoutService _layoutService;
        private ISettingService _settingService;

        public FooterViewComponent(ILayoutService layoutService,
                                   ISettingService settingService)
        {
            _layoutService = layoutService;
            _settingService = settingService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _layoutService.GetFooterDataAsync(HttpContext.User);
            return View(model);
        }
    }
}
