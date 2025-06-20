using Microsoft.EntityFrameworkCore;
using CityHotelGarageAPI.Repository.Data;
using CityHotelGarageAPI.Repository.Interfaces;
using CityHotelGarageAPI.Repository.Models;

namespace CityHotelGarageAPI.Repository.Repositories;

public class CityRepository : BaseRepository<City>, ICityRepository
{
    public CityRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<City>> GetCitiesWithHotelsAsync()
    {
        return await _context.Cities
            .Include(c => c.Hotels)
            .ToListAsync();
    }

    public async Task<City?> GetCityWithHotelsAsync(int id)
    {
        return await _context.Cities
            .Include(c => c.Hotels)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}