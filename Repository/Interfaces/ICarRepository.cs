using CityHotelGarageAPI.Repository.Models;

namespace CityHotelGarageAPI.Repository.Interfaces;

public interface ICarRepository : IBaseRepository<Car>
{
    Task<IEnumerable<Car>> GetCarsWithDetailsAsync();
    Task<Car?> GetCarWithDetailsAsync(int id);
    Task<Car?> GetCarByLicensePlateAsync(string licensePlate);
    Task<IEnumerable<Car>> GetCarsByGarageAsync(int garageId);
    Task<bool> IsLicensePlateExistsAsync(string licensePlate);
}