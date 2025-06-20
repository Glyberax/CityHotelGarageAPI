using CityHotelGarageAPI.Repository.Models;

namespace CityHotelGarageAPI.Repository.Interfaces;

public interface IGarageRepository : IBaseRepository<Garage>
{
    Task<IEnumerable<Garage>> GetGaragesWithDetailsAsync();
    Task<Garage?> GetGarageWithDetailsAsync(int id);
    Task<IEnumerable<Garage>> GetGaragesByHotelAsync(int hotelId);
    Task<int> GetAvailableSpacesAsync(int garageId);
}