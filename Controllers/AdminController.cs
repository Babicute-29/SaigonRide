using Microsoft.AspNetCore.Mvc;
using SaigonRide.Models;
using SaigonRide.Services;
using SaigonRide.Data; // Thêm cái này để dùng AppDbContext

namespace SaigonRide.Controllers
{
    public class AdminController : Controller
    {
        private readonly StationService _stationService;
        private readonly VehicleService _vehicleService;
        private readonly AppDbContext _context; // Khai báo thêm Context để quản lý RentingHistory

        // Cập nhật Constructor: Thêm AppDbContext vào
        public AdminController(StationService stationService, VehicleService vehicleService, AppDbContext context)
        {
            _stationService = stationService;
            _vehicleService = vehicleService;
            _context = context;
        }

        public IActionResult Dashboard()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return RedirectToAction("Login", "Auth");
            return View();
        }

        #region STATION MANAGEMENT
        // ... Giữ nguyên phần Station Nhu đã có ...
        public IActionResult ManageStation()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return RedirectToAction("Login", "Auth");
            var stations = _stationService.GetList();
            return View(stations);
        }

        [HttpPost]
        public IActionResult CreateStation(Station s)
        {
            var isExist = _stationService.GetList().Any(x => x.StationName.ToLower().Trim() == s.StationName.ToLower().Trim());
            if (isExist) { TempData["Error"] = "This station name already exists!"; return RedirectToAction("ManageStation"); }
            if (ModelState.IsValid) { _stationService.Create(s); TempData["Success"] = "New station added!"; }
            return RedirectToAction("ManageStation");
        }

        public IActionResult DeleteStation(int id) { _stationService.Remove(id); TempData["Success"] = "Deleted!"; return RedirectToAction("ManageStation"); }

        [HttpPost]
        public IActionResult EditStation(Station s) { if (ModelState.IsValid) _stationService.Edit(s); return RedirectToAction("ManageStation"); }
        #endregion

        #region VEHICLE MANAGEMENT
        public IActionResult ManageVehicle()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return RedirectToAction("Login", "Auth");
            ViewBag.Stations = _stationService.GetList();
            var vehicles = _vehicleService.GetList();
            return View(vehicles);
        }

        [HttpPost]
        public IActionResult CreateVehicle(Vehicle v)
        {
            var allVehicles = _vehicleService.GetList();
            string prefix = v.Type == "E-Scooter" ? "ES" : "SD";
            int count = allVehicles.Count(x => x.Type == v.Type) + 1;
            v.VehicleName = $"{prefix}{count:D2}";
            v.Status = "Available"; // Luôn mặc định xe mới là sẵn sàng

            if (ModelState.IsValid)
            {
                _vehicleService.Create(v);
                TempData["Success"] = $"Created {v.VehicleName} successfully!";
            }
            return RedirectToAction("ManageVehicle");
        }

        [HttpPost]
        public IActionResult EditVehicle(Vehicle v)
        {
            if (ModelState.IsValid) { _vehicleService.Edit(v); TempData["Success"] = "Updated!"; }
            return RedirectToAction("ManageVehicle");
        }

        public IActionResult DeleteVehicle(int id)
        {
            var vehicle = _vehicleService.GetList().FirstOrDefault(v => v.Id == id);
            if (vehicle != null)
            {
                if (vehicle.Status == "Rented")
                {
                    TempData["Error"] = $"Cannot delete {vehicle.VehicleName} - It's currently rented!";
                    return RedirectToAction("ManageVehicle");
                }
                _vehicleService.Remove(id);
                TempData["Success"] = "Deleted!";
            }
            return RedirectToAction("ManageVehicle");
        }
        #endregion

        #region USER RENTAL MANAGEMENT (HOÀN THIỆN CÁI SỐ 3)

        public IActionResult ManageUser()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return RedirectToAction("Login", "Auth");

            // 1. Lấy toàn bộ lịch sử thuê
            var histories = _context.RentingHistories.OrderByDescending(h => h.StartTime).ToList();

            // 2. Truyền thêm dữ liệu bổ trợ để hiển thị tên thay vì ID
            ViewBag.Users = _context.Users.ToList();
            ViewBag.Vehicles = _vehicleService.GetList();
            ViewBag.Stations = _stationService.GetList();

            return View(histories);
        }

        #endregion
    }
}