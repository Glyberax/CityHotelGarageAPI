using CityHotelGarageAPI.Operations.DTOs;
using CityHotelGarageAPI.Operations.Interfaces;
using CityHotelGarageAPI.Operations.Results;
using CityHotelGarageAPI.Repository.Interfaces;
using CityHotelGarageAPI.Repository.Models;

namespace CityHotelGarageAPI.Operations.Services;

public class CityService : ICityService
{
    private readonly ICityRepository _cityRepository;

    public CityService(ICityRepository cityRepository)
    {
        _cityRepository = cityRepository;
    }

    public async Task<ServiceResult<IEnumerable<CityDto>>> GetAllCitiesAsync()
    {
        try
        {
            var cities = await _cityRepository.GetCitiesWithHotelsAsync();
            var cityDtos = cities.Select(c => new CityDto
            {
                Id = c.Id,
                Name = c.Name,
                Population = c.Population,
                CreatedDate = c.CreatedDate,
                HotelCount = c.Hotels.Count
            });

            return ServiceResult<IEnumerable<CityDto>>.Success(cityDtos, "Şehirler başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return ServiceResult<IEnumerable<CityDto>>.Failure($"Şehirler getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<ServiceResult<CityDto>> GetCityByIdAsync(int id)
    {
        try
        {
            var city = await _cityRepository.GetCityWithHotelsAsync(id);
            if (city == null)
            {
                return ServiceResult<CityDto>.Failure("Şehir bulunamadı.");
            }

            var cityDto = new CityDto
            {
                Id = city.Id,
                Name = city.Name,
                Population = city.Population,
                CreatedDate = city.CreatedDate,
                HotelCount = city.Hotels.Count
            };

            return ServiceResult<CityDto>.Success(cityDto, "Şehir başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return ServiceResult<CityDto>.Failure($"Şehir getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<ServiceResult<CityDto>> CreateCityAsync(CityCreateDto cityDto)
    {
        try
        {
            var city = new City
            {
                Name = cityDto.Name,
                Population = cityDto.Population,
                CreatedDate = DateTime.UtcNow
            };

            var createdCity = await _cityRepository.AddAsync(city);

            var resultDto = new CityDto
            {
                Id = createdCity.Id,
                Name = createdCity.Name,
                Population = createdCity.Population,
                CreatedDate = createdCity.CreatedDate,
                HotelCount = 0
            };

            return ServiceResult<CityDto>.Success(resultDto, "Şehir başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            return ServiceResult<CityDto>.Failure($"Şehir oluşturulurken hata oluştu: {ex.Message}");
        }
    }

    public async Task<ServiceResult<CityDto>> UpdateCityAsync(int id, CityCreateDto cityDto)
    {
        try
        {
            var existingCity = await _cityRepository.GetByIdAsync(id);
            if (existingCity == null)
            {
                return ServiceResult<CityDto>.Failure("Güncellenecek şehir bulunamadı.");
            }

            existingCity.Name = cityDto.Name;
            existingCity.Population = cityDto.Population;

            var updatedCity = await _cityRepository.UpdateAsync(existingCity);

            var resultDto = new CityDto
            {
                Id = updatedCity.Id,
                Name = updatedCity.Name,
                Population = updatedCity.Population,
                CreatedDate = updatedCity.CreatedDate,
                HotelCount = 0 // Bu bilgiyi almak için ekstra sorgu gerekebilir
            };

            return ServiceResult<CityDto>.Success(resultDto, "Şehir başarıyla güncellendi.");
        }
        catch (Exception ex)
        {
            return ServiceResult<CityDto>.Failure($"Şehir güncellenirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<ServiceResult> DeleteCityAsync(int id)
    {
        try
        {
            var exists = await _cityRepository.ExistsAsync(id);
            if (!exists)
            {
                return ServiceResult.Failure("Silinecek şehir bulunamadı.");
            }

            var deleted = await _cityRepository.DeleteAsync(id);
            if (!deleted)
            {
                return ServiceResult.Failure("Şehir silinirken hata oluştu.");
            }

            return ServiceResult.Success("Şehir başarıyla silindi.");
        }
        catch (Exception ex)
        {
            return ServiceResult.Failure($"Şehir silinirken hata oluştu: {ex.Message}");
        }
    }
}