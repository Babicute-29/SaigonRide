using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaigonRide.Data;
using SaigonRide.Models;
using SaigonRide.Services;

namespace SaigonRide.Controllers
{
    public class AdminController : Controller
    {
        private readonly StationService _stationService;
        private readonly VehicleService _vehicleService;
        private readonly AppDbContext _context;

        public AdminController(StationService stationService, VehicleService vehicleService, AppDbContext context)
        {
            _stationService = stationService;
            _vehicleService = vehicleService;
            _context = context;
        }

        // Hàm tiện ích để kiểm tra quyền Admin nhanh
        private bool IsAdmin() => HttpContext.Session.GetString("Role") == "Admin";

        #region DASHBOARD
        public IActionResult Dashboard()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            ViewBag.AdminName = HttpContext.Session.GetString("Username") ?? "Administrator";
            ViewBag.AdminEmail = "admin@saigonride.com";

            ViewBag.TotalVehicles = _context.Vehicles.Count();
            ViewBag.TotalUsers = _context.Users.Count(u => u.Role == "User");
            ViewBag.TotalStations = _context.Stations.Count();
            ViewBag.TotalRentals = _context.RentingHistories.Count();
            ViewBag.PendingSupport = _context.SupportReports.Count(r => r.Status == "Pending");

            return View();
        }
        #endregion

        #region STATION MANAGEMENT
        public IActionResult ManageStation()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            return View(_stationService.GetList());
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
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            ViewBag.Stations = _stationService.GetList();
            return View(_vehicleService.GetList());
        }

        [HttpPost]
        public IActionResult CreateVehicle(Vehicle v)
        {
            var allVehicles = _vehicleService.GetList();
            string prefix = v.Type == "E-Scooter" ? "ES" : "SD";
            int count = allVehicles.Count(x => x.Type == v.Type) + 1;
            v.VehicleName = $"{prefix}{count:D2}";
            v.Status = "Available";

            if (ModelState.IsValid) { _vehicleService.Create(v); TempData["Success"] = $"Created {v.VehicleName} successfully!"; }
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
                if (vehicle.Status == "Rented") { TempData["Error"] = "Cannot delete rented vehicle!"; return RedirectToAction("ManageVehicle"); }
                _vehicleService.Remove(id);
                TempData["Success"] = "Deleted!";
            }
            return RedirectToAction("ManageVehicle");
        }
        #endregion

        #region SUPPORT & REPORT MANAGEMENT (ĐÃ CẬP NHẬT THÊM XÓA)

        public IActionResult SupportManagement()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            var reports = _context.SupportReports
                .Include(r => r.User)
                .Include(r => r.Vehicle)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            return View(reports);
        }

        public IActionResult SupportDetail(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            var report = _context.SupportReports
                .Include(r => r.User)
                .Include(r => r.Vehicle)
                .FirstOrDefault(r => r.Id == id);

            if (report == null) return NotFound();
            return View(report);
        }

        [HttpPost]
        public IActionResult ReplySupportDetail(int Id, string AdminReply)
        {
            var report = _context.SupportReports.Find(Id);
            if (report != null)
            {
                report.AdminReply = AdminReply;
                report.Status = "Resolved";
                _context.SaveChanges();
                TempData["Success"] = "Reply sent successfully!";
            }
            return RedirectToAction("SupportDetail", new { id = Id });
        }

        // CẬP NHẬT MỚI: Xóa yêu cầu hỗ trợ khỏi Database
        [HttpPost]
        public IActionResult DeleteSupport(int id)
        {
            if (!IsAdmin()) return Json(new { success = false });

            var report = _context.SupportReports.Find(id);
            if (report != null)
            {
                _context.SupportReports.Remove(report);
                _context.SaveChanges();
                TempData["Success"] = "Support request deleted!";
            }
            return RedirectToAction("SupportManagement");
        }

        #endregion

        #region USER RENTAL HISTORY
        public IActionResult ManageUser()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            var histories = _context.RentingHistories.OrderByDescending(h => h.StartTime).ToList();
            ViewBag.Users = _context.Users.ToList();
            ViewBag.Vehicles = _vehicleService.GetList();
            ViewBag.Stations = _stationService.GetList();

            return View(histories);
        }
        #endregion
    }
}