using Microsoft.EntityFrameworkCore;
using SmartParkingAPI.Models;

namespace SmartParkingAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ParkingLot> ParkingLots { get; set; }
        public DbSet<ParkingSpot> ParkingSpots { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ParkingLot>()
                .Property(p => p.PricePerHour)
                .HasPrecision(18, 2);
        }
    }
}