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

        // 1. Danh sách xe sẵn sàng thuê
        public IActionResult RentVehicle()
        {
            var availableVehicles = _vehicleService.GetList().Where(v => v.Status == "Available").ToList();
            ViewBag.Stations = _stationService.GetList();
            return View(availableVehicles);
        }

        // 2. Xác nhận thuê xe
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

        // 3. Xem xe đang thuê hiện tại (Có đồng hồ đếm)
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

        // 4. Trả xe và ghi nhận trạm kết thúc chặng
        [HttpPost]
        public IActionResult ReturnVehicle(int historyId)
        {
            var history = _context.RentingHistories
                .Include(h => h.Vehicle)
                .FirstOrDefault(h => h.Id == historyId);

            if (history == null || history.Status != "In Progress")
                return RedirectToAction("Index", "Home");

            history.EndTime = DateTime.Now;
            history.Status = "Completed";
            history.ReturnStationId = history.PickupStationId;

            // --- LOGIC TÍNH TIỀN THEO ĐỀ BÀI ---
            TimeSpan duration = history.EndTime.Value - history.StartTime;
            double totalMinutes = Math.Max(1, duration.TotalMinutes); // Ít nhất tính 1 phút

            // 1. Xác định đơn giá dựa trên loại xe
            decimal ratePerMinute = (history.Vehicle?.Type == "E-Scooter") ? 1500m : 500m;

            // 2. Tính giá gốc
            decimal basePrice = (decimal)totalMinutes * ratePerMinute;

            // 3. Áp dụng giảm giá 15% (Location Discount)
            decimal discount = basePrice * 0.15m;
            history.TotalPrice = basePrice - discount;

            if (history.Vehicle != null) history.Vehicle.Status = "Available";

            _context.SaveChanges();
            return RedirectToAction("RentalHistory");
        }

        // 5. Lịch sử các chặng đã đi
        public IActionResult RentalHistory()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var histories = _context.RentingHistories
                .Include(h => h.Vehicle)
                .Include(h => h.PickupStation)
                .Include(h => h.ReturnStation) // Kéo thêm dữ liệu trạm trả để hiện chặng đường
                .Where(h => h.UserId == userId && h.Status == "Completed")
                .OrderByDescending(h => h.EndTime)
                .ToList();

            return View(histories);
        }
    }
}