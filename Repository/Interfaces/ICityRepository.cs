using CityHotelGarageAPI.Repository.Models;

namespace CityHotelGarageAPI.Repository.Interfaces;

public interface ICityRepository : IBaseRepository<City>
{
    Task<IEnumerable<City>> GetCitiesWithHotelsAsync();
    Task<City?> GetCityWithHotelsAsync(int id);
}