using CityHotelGarageAPI.Operations.DTOs;
using CityHotelGarageAPI.Operations.Results;

namespace CityHotelGarageAPI.Operations.Interfaces;

public interface ICityService
{
    Task<ServiceResult<IEnumerable<CityDto>>> GetAllCitiesAsync();
    Task<ServiceResult<CityDto>> GetCityByIdAsync(int id);
    Task<ServiceResult<CityDto>> CreateCityAsync(CityCreateDto cityDto);
    Task<ServiceResult<CityDto>> UpdateCityAsync(int id, CityCreateDto cityDto);
    Task<ServiceResult> DeleteCityAsync(int id);
}