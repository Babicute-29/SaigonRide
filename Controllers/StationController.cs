using Microsoft.AspNetCore.Mvc;

namespace SaigonRide.Controllers
{
    public class StationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
