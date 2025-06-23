using CityHotelGarageAPI.Repository.Models;

namespace CityHotelGarageAPI.Repository.Interfaces;

public interface ICityRepository : IBaseRepository<City>
{
    IQueryable<City> GetCitiesWithHotels();
    Task<City?> GetCityWithHotelsAsync(int id);
}