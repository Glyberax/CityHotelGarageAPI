using Microsoft.EntityFrameworkCore;
using CityHotelGarageAPI.Repository.Data;
using CityHotelGarageAPI.Repository.Interfaces;
using CityHotelGarageAPI.Repository.Models;

namespace CityHotelGarageAPI.Repository.Repositories;

public class HotelRepository : BaseRepository<Hotel>, IHotelRepository
{
    public HotelRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Hotel>> GetHotelsWithDetailsAsync()
    {
        return await _context.Hotels
            .Include(h => h.City)
            .Include(h => h.Garages)
            .ToListAsync();
    }

    public async Task<Hotel?> GetHotelWithDetailsAsync(int id)
    {
        return await _context.Hotels
            .Include(h => h.City)
            .Include(h => h.Garages)
            .FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task<IEnumerable<Hotel>> GetHotelsByCityAsync(int cityId)
    {
        return await _context.Hotels
            .Include(h => h.City)
            .Include(h => h.Garages)
            .Where(h => h.CityId == cityId)
            .ToListAsync();
    }
}