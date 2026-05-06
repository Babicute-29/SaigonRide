using Microsoft.AspNetCore.Mvc;

namespace SaigonRide.Controllers
{
    public class VehicleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
