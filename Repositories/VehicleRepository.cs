using SaigonRide.Data;
using SaigonRide.Models;

namespace SaigonRide.Repositories
{
    public class VehicleRepository
    {
        private readonly AppDbContext _context;
        public VehicleRepository(AppDbContext context) { _context = context; }

        public List<Vehicle> GetAll() => _context.Vehicles.ToList();

        public void Add(Vehicle v)
        {
            _context.Vehicles.Add(v);
            _context.SaveChanges();
        }

        public void Update(Vehicle v)
        {
            var existing = _context.Vehicles.Find(v.Id);
            if (existing != null)
            {
                existing.VehicleName = v.VehicleName;
                existing.Type = v.Type;
                existing.Status = v.Status;
                existing.StationId = v.StationId;
                _context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var v = _context.Vehicles.Find(id);
            if (v != null)
            {
                _context.Vehicles.Remove(v);
                _context.SaveChanges();
            }
        }
    }
}