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
    }
}