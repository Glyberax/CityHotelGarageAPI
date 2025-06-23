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

    public IQueryable<Car> GetCarsWithDetails()
    {
        return _context.Cars
            .Include(c => c.Garage)
            .ThenInclude(g => g!.Hotel)
            .ThenInclude(h => h.City);
    }

    public async Task<Car?> GetCarWithDetailsAsync(int id)
    {
        return await GetCarsWithDetails()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Car?> GetCarByLicensePlateAsync(string licensePlate)
    {
        return await GetCarsWithDetails()
            .FirstOrDefaultAsync(c => c.LicensePlate == licensePlate);
    }

    public IQueryable<Car> GetCarsByGarage(int garageId)
    {
        return GetCarsWithDetails()
            .Where(c => c.GarageId == garageId);
    }

    public async Task<bool> IsLicensePlateExistsAsync(string licensePlate)
    {
        return await _context.Cars
            .AnyAsync(c => c.LicensePlate == licensePlate);
    }
}