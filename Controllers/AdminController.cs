using Microsoft.AspNetCore.Mvc;

namespace SaigonRide.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }
    }
}