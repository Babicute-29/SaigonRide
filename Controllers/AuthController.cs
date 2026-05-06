using Microsoft.AspNetCore.Mvc;
using SaigonRide.Services;

namespace SaigonRide.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _service;

        public AuthController(AuthService service)
        {
            _service = service;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _service.Login(email, password);

            if (user == null)
            {
                ViewBag.Error = "Failed Email or Password!";
                return View();
            }

            
            HttpContext.Session.SetInt32("UserId", user.Id);

            HttpContext.Session.SetString("Email", user.Email ?? "");
            HttpContext.Session.SetString("FullName", user.FullName ?? "User");
            HttpContext.Session.SetString("Phone", user.Phone ?? "N/A");
            HttpContext.Session.SetString("Country", user.Country ?? "Vietnam");

            if (email.EndsWith(".admin@gmail.com"))
            {
                HttpContext.Session.SetString("Role", "Admin");
                return RedirectToAction("Dashboard", "Admin");
            }
            else
            {
                HttpContext.Session.SetString("Role", "User");
                
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(SaigonRide.Models.User user)
        {
            if (user.Country != "Vietnam" && string.IsNullOrEmpty(user.Passport))
            {
                ViewBag.Error = "If not Vietnam, passport is required!";
                return View();
            }

            _service.Register(user);

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}