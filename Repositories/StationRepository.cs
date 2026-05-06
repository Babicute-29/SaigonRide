using SaigonRide.Data;
using SaigonRide.Models;
using Microsoft.EntityFrameworkCore; // Thêm dòng này để dùng EntityState nếu cần

namespace SaigonRide.Repositories
{
    public class StationRepository
    {
        private readonly AppDbContext _context;
        public StationRepository(AppDbContext context) { _context = context; }

        public List<Station> GetAll() => _context.Stations.ToList();

        public Station? GetById(int id) => _context.Stations.Find(id);

        public void Add(Station station)
        {
            _context.Stations.Add(station);
            _context.SaveChanges();
        }

        // Sửa lại hàm Update này cho chắc chắn nè Như
        public void Update(Station station)
        {
            var existingStation = _context.Stations.Find(station.Id); 
            if (existingStation != null)
            {
                // Cập nhật từng thuộc tính từ form vào database
                existingStation.StationName = station.StationName; 
                existingStation.Location = station.Location; 
                existingStation.Capacity = station.Capacity; 
                existingStation.Status = station.Status; 


                _context.SaveChanges(); 
            }
        }

        public void Delete(int id)
        {
            var s = _context.Stations.Find(id); 
            if (s != null)
            {
                _context.Stations.Remove(s); 
                _context.SaveChanges();
            }
        }
    }
}