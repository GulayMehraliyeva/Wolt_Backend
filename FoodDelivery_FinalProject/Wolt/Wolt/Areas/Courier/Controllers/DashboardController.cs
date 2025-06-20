using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Wolt.Areas.Courier.Controllers
{
    [Area("Courier")]
    [Authorize(Roles = "Courier")]
    public class DashboardController : Controller
    {
        [Authorize(Roles = "Courier")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
