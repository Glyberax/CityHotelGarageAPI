using CityHotelGarageAPI.Operations.DTOs;
using CityHotelGarageAPI.Operations.Results;

namespace CityHotelGarageAPI.Operations.Interfaces;

public interface ICarService
{
    Task<ServiceResult<IEnumerable<CarDto>>> GetAllCarsAsync();
    Task<ServiceResult<CarDto>> GetCarByIdAsync(int id);
    Task<ServiceResult<CarDto>> GetCarByLicensePlateAsync(string licensePlate);
    Task<ServiceResult<CarDto>> ParkCarAsync(CarCreateDto carDto);
    Task<ServiceResult<CarDto>> UpdateCarAsync(int id, CarCreateDto carDto);
    Task<ServiceResult> RemoveCarAsync(int id);
}