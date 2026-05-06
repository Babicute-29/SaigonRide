using SaigonRide.Models;
using SaigonRide.Repositories;

namespace SaigonRide.Services
{
    public class VehicleService
    {
        private readonly VehicleRepository _repo;
        public VehicleService(VehicleRepository repo) { _repo = repo; }

        public List<Vehicle> GetList() => _repo.GetAll();
        public void Create(Vehicle v) => _repo.Add(v);
        public void Edit(Vehicle v) => _repo.Update(v);
        public void Remove(int id) => _repo.Delete(id);
    }
}