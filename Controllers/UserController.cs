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

            // Cập nhật kết thúc hành trình
            history.EndTime = DateTime.Now;
            history.Status = "Completed";

            // Ghi nhận trạm trả (Mặc định là trạm xe đang đậu lúc trả)
            history.ReturnStationId = history.PickupStationId;

            // Tính tiền theo block 30 phút
            TimeSpan duration = history.EndTime.Value - history.StartTime;
            int blocks = (int)Math.Ceiling(duration.TotalMinutes / 30);
            if (blocks < 1) blocks = 1;
            history.TotalPrice = blocks * 10000;

            if (history.Vehicle != null)
            {
                history.Vehicle.Status = "Available";
            }

            _context.SaveChanges();

            return RedirectToAction("RentalHistory"); // Trả xong xem lịch sử luôn cho tiện
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