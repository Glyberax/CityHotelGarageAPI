using Microsoft.EntityFrameworkCore;

namespace CityHotelGarageAPI;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<City> Cities { get; set; }
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Garage> Garages { get; set; }
    public DbSet<Car> Cars { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // City -> Hotel (One-to-Many)
        modelBuilder.Entity<Hotel>()
            .HasOne(h => h.City)
            .WithMany(c => c.Hotels)
            .HasForeignKey(h => h.CityId)
            .OnDelete(DeleteBehavior.Cascade);

        // Hotel -> Garage (One-to-Many)
        modelBuilder.Entity<Garage>()
            .HasOne(g => g.Hotel)
            .WithMany(h => h.Garages)
            .HasForeignKey(g => g.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        // Garage -> Car (One-to-Many)
        modelBuilder.Entity<Car>()
            .HasOne(c => c.Garage)
            .WithMany(g => g.Cars)
            .HasForeignKey(c => c.GarageId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<Car>()
            .HasIndex(c => c.LicensePlate)
            .IsUnique();
    }
}