using Microsoft.AspNetCore.Mvc;
using SaigonRide.Data;
using SaigonRide.Models;
using SaigonRide.Services;
using Microsoft.EntityFrameworkCore;

namespace SaigonRide.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly VehicleService _vehicleService;
        private readonly StationService _stationService;

        public UserController(AppDbContext context, VehicleService vehicleService, StationService stationService)
        {
            _context = context;
            _vehicleService = vehicleService;
            _stationService = stationService;
        }

        // 1. Trang cá nhân (Cập nhật để hiện Support History)
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            // Lấy thông tin user (để hiện FullName, Email... như trong image_6a9bea.jpg)
            var user = _context.Users.Find(userId);
            ViewBag.User = user;

            // Lấy danh sách hỗ trợ của riêng user này
            ViewBag.MyReports = _context.SupportReports
                .Include(r => r.Vehicle)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            return View();
        }

        // 2. Danh sách xe sẵn sàng thuê
        public IActionResult RentVehicle()
        {
            var availableVehicles = _vehicleService.GetList().Where(v => v.Status == "Available").ToList();
            ViewBag.Stations = _stationService.GetList();
            return View(availableVehicles);
        }

        // 3. Xác nhận thuê xe
        [HttpPost]
        public IActionResult ConfirmRent(int vehicleId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var vehicle = _context.Vehicles.Find(vehicleId);
            if (vehicle == null || vehicle.Status != "Available")
            {
                TempData["Error"] = "Vehicle is not available!";
                return RedirectToAction("RentVehicle");
            }

            vehicle.Status = "Rented";

            var rentHistory = new RentingHistory
            {
                UserId = userId.Value,
                VehicleId = vehicleId,
                PickupStationId = vehicle.StationId,
                StartTime = DateTime.Now,
                Status = "In Progress"
            };

            _context.RentingHistories.Add(rentHistory);
            _context.SaveChanges();

            return RedirectToAction("MyRental");
        }

        // 4. Xem xe đang thuê hiện tại
        public IActionResult MyRental()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var currentRent = _context.RentingHistories
                .Include(h => h.Vehicle)
                .Include(h => h.PickupStation)
                .FirstOrDefault(h => h.UserId == userId && h.Status == "In Progress");

            return View(currentRent);
        }

        // 5. Trả xe và tính tiền (Giữ nguyên logic giảm giá 15%)
        [HttpPost]
        public IActionResult ReturnVehicle(int historyId)
        {
            var history = _context.RentingHistories
                .Include(h => h.Vehicle)
                .FirstOrDefault(h => h.Id == historyId);

            if (history == null || history.Status != "In Progress")
                return RedirectToAction("Index");

            history.EndTime = DateTime.Now;
            history.Status = "Completed";
            history.ReturnStationId = history.PickupStationId;

            TimeSpan duration = history.EndTime.Value - history.StartTime;
            double totalMinutes = Math.Max(1, duration.TotalMinutes);

            decimal ratePerMinute = (history.Vehicle?.Type == "E-Scooter") ? 1500m : 500m;
            decimal basePrice = (decimal)totalMinutes * ratePerMinute;
            decimal discount = basePrice * 0.15m;
            history.TotalPrice = basePrice - discount;

            if (history.Vehicle != null) history.Vehicle.Status = "Available";

            _context.SaveChanges();
            return RedirectToAction("RentalHistory");
        }

        // 6. Lịch sử các chặng đã đi
        public IActionResult RentalHistory()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var histories = _context.RentingHistories
                .Include(h => h.Vehicle)
                .Include(h => h.PickupStation)
                .Include(h => h.ReturnStation)
                .Where(h => h.UserId == userId && h.Status == "Completed")
                .OrderByDescending(h => h.EndTime)
                .ToList();

            return View(histories);
        }

        // --- PHẦN HỖ TRỢ (SUPPORT) ---

        // 7. Gửi yêu cầu hỗ trợ mới
        [HttpPost]
        public IActionResult SendSupport(int? VehicleId, string Message)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var newReport = new SupportReport
            {
                UserId = userId.Value,
                VehicleId = VehicleId,
                Message = Message,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            _context.SupportReports.Add(newReport);
            _context.SaveChanges();

            TempData["Success"] = "Your report has been sent. We will get back to you soon!";
            return RedirectToAction("Index", "Home");
        }
    }
}