using CityHotelGarageAPI.Repository.Models;

namespace CityHotelGarageAPI.Repository.Interfaces;

public interface IHotelRepository : IBaseRepository<Hotel>
{
    Task<IEnumerable<Hotel>> GetHotelsWithDetailsAsync();
    Task<Hotel?> GetHotelWithDetailsAsync(int id);
    Task<IEnumerable<Hotel>> GetHotelsByCityAsync(int cityId);
}