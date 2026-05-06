using SaigonRide.Models;
using SaigonRide.Repositories;

namespace SaigonRide.Services
{
    public class StationService
    {
        private readonly StationRepository _repo;
        public StationService(StationRepository repo) { _repo = repo; }

        public List<Station> GetList() => _repo.GetAll();
        public Station? GetDetail(int id) => _repo.GetById(id);
        public void Create(Station s) => _repo.Add(s);
        public void Edit(Station s) => _repo.Update(s);
        public void Remove(int id) => _repo.Delete(id);
    }
}