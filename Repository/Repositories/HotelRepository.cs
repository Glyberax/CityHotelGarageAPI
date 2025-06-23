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

    public IQueryable<Hotel> GetHotelsWithDetails()
    {
        return _context.Hotels
            .Include(h => h.City)
            .Include(h => h.Garages);
    }

    public async Task<Hotel?> GetHotelWithDetailsAsync(int id)
    {
        return await GetHotelsWithDetails()
            .FirstOrDefaultAsync(h => h.Id == id);
    }

    public IQueryable<Hotel> GetHotelsByCity(int cityId)
    {
        return GetHotelsWithDetails()
            .Where(h => h.CityId == cityId);
    }
}