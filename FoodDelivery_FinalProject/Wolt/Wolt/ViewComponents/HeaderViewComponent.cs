using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;

namespace Wolt.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly ILayoutService _layoutService;
        private ISettingService _settingService;

        public HeaderViewComponent(ILayoutService layoutService, 
                                   ISettingService settingService)
        {
            _layoutService = layoutService;
            _settingService = settingService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _layoutService.GetHeaderDataAsync(HttpContext.User);
            return View(model);
        }
    }
}
