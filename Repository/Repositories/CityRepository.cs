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

    public IQueryable<City> GetCitiesWithHotels()
    {
        return _context.Cities
            .Include(c => c.Hotels);
    }

    public async Task<City?> GetCityWithHotelsAsync(int id)
    {
        return await GetCitiesWithHotels()
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}