using Microsoft.AspNetCore.Mvc;
using SaigonRide.Models;
using System.Diagnostics;

namespace SaigonRide.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.FullName = HttpContext.Session.GetString("FullName");
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.Phone = HttpContext.Session.GetString("Phone");
            ViewBag.Country = HttpContext.Session.GetString("Country");

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
