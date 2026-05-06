using Microsoft.EntityFrameworkCore;
using SaigonRide.Models;

namespace SaigonRide.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Station> Stations { get; set; }

        
        public DbSet<Vehicle> Vehicles { get; set; }

        public DbSet<RentingHistory> RentingHistories { get; set; }

        public DbSet<SupportReport> SupportReports { get; set; }
    }
}