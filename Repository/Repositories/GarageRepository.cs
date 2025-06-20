using Microsoft.EntityFrameworkCore;
using CityHotelGarageAPI.Repository.Data;
using CityHotelGarageAPI.Repository.Interfaces;
using CityHotelGarageAPI.Repository.Models;

namespace CityHotelGarageAPI.Repository.Repositories;

public class GarageRepository : BaseRepository<Garage>, IGarageRepository
{
    public GarageRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Garage>> GetGaragesWithDetailsAsync()
    {
        return await _context.Garages
            .Include(g => g.Hotel)
            .ThenInclude(h => h.City)
            .Include(g => g.Cars)
            .ToListAsync();
    }

    public async Task<Garage?> GetGarageWithDetailsAsync(int id)
    {
        return await _context.Garages
            .Include(g => g.Hotel)
            .ThenInclude(h => h.City)
            .Include(g => g.Cars)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<IEnumerable<Garage>> GetGaragesByHotelAsync(int hotelId)
    {
        return await _context.Garages
            .Include(g => g.Hotel)
            .ThenInclude(h => h.City)
            .Include(g => g.Cars)
            .Where(g => g.HotelId == hotelId)
            .ToListAsync();
    }

    public async Task<int> GetAvailableSpacesAsync(int garageId)
    {
        var garage = await _context.Garages
            .Include(g => g.Cars)
            .FirstOrDefaultAsync(g => g.Id == garageId);
        
        if (garage == null) return 0;
        
        return garage.Capacity - garage.Cars.Count;
    }
}