using Microsoft.EntityFrameworkCore;
using CityHotelGarageAPI.Repository.Data;
using CityHotelGarageAPI.Repository.Interfaces;
using CityHotelGarageAPI.Repository.Models;

namespace CityHotelGarageAPI.Repository.Repositories;

public class CarRepository : BaseRepository<Models.Car>, ICarRepository
{
    public CarRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Car>> GetCarsWithDetailsAsync()
    {
        return await _context.Cars
            .Include(c => c.Garage)
            .ThenInclude(g => g!.Hotel)
            .ThenInclude(h => h.City)
            .ToListAsync();
    }

    public async Task<Car?> GetCarWithDetailsAsync(int id)
    {
        return await _context.Cars
            .Include(c => c.Garage)
            .ThenInclude(g => g!.Hotel)
            .ThenInclude(h => h.City)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Car?> GetCarByLicensePlateAsync(string licensePlate)
    {
        return await _context.Cars
            .Include(c => c.Garage)
            .ThenInclude(g => g!.Hotel)
            .ThenInclude(h => h.City)
            .FirstOrDefaultAsync(c => c.LicensePlate == licensePlate);
    }

    public async Task<IEnumerable<Car>> GetCarsByGarageAsync(int garageId)
    {
        return await _context.Cars
            .Include(c => c.Garage)
            .ThenInclude(g => g!.Hotel)
            .ThenInclude(h => h.City)
            .Where(c => c.GarageId == garageId)
            .ToListAsync();
    }

    public async Task<bool> IsLicensePlateExistsAsync(string licensePlate)
    {
        return await _context.Cars
            .AnyAsync(c => c.LicensePlate == licensePlate);
    }
}