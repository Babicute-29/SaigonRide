using Microsoft.AspNetCore.Mvc;
using SaigonRide.Data; // Cần thiết để dùng AppDbContext
using SaigonRide.Models;
using Microsoft.EntityFrameworkCore; // Cần thiết để dùng .Include()
using System.Diagnostics;

namespace SaigonRide.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context; // Khai báo context để lấy dữ liệu Support

        // Cập nhật Constructor để Inject AppDbContext
        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            // 1. Kiểm tra Login
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            // 2. Lấy thông tin User đầy đủ từ DB để hiện lên Profile Card
            var user = _context.Users.Find(userId);
            ViewBag.User = user;

            // 3. Lấy lịch sử hỗ trợ để hiện lên bảng (Phần quan trọng để hiện tên)
            ViewBag.MyReports = _context.SupportReports
                .Include(r => r.User)    // Lệnh này giúp lấy FullName thay vì số ID
                .Include(r => r.Vehicle)
                .Where(r => r.UserId == userId.Value)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

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