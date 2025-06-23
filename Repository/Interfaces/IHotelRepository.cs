using CityHotelGarageAPI.Repository.Models;

namespace CityHotelGarageAPI.Repository.Interfaces;

public interface IHotelRepository : IBaseRepository<Hotel>
{
    IQueryable<Hotel> GetHotelsWithDetails();
    Task<Hotel?> GetHotelWithDetailsAsync(int id);
    IQueryable<Hotel> GetHotelsByCity(int cityId);
}