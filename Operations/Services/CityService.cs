using Microsoft.EntityFrameworkCore;
using CityHotelGarageAPI.Operations.DTOs;
using CityHotelGarageAPI.Operations.Extensions;
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
            var cityDtos = await _cityRepository.GetCitiesWithHotels()
                .ProjectToCityDto()
                .ToListAsync();

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
            var cityDto = await _cityRepository.GetCitiesWithHotels()
                .Where(c => c.Id == id)
                .ProjectToCityDto()
                .FirstOrDefaultAsync();

            if (cityDto == null)
            {
                return ServiceResult<CityDto>.Failure("Şehir bulunamadı.");
            }

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

            // Projection ile detaylı bilgiyi al
            var resultDto = await _cityRepository.GetCitiesWithHotels()
                .Where(c => c.Id == createdCity.Id)
                .ProjectToCityDto()
                .FirstAsync();

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

            await _cityRepository.UpdateAsync(existingCity);

            // Projection ile güncellenmiş veriyi al
            var resultDto = await _cityRepository.GetCitiesWithHotels()
                .Where(c => c.Id == id)
                .ProjectToCityDto()
                .FirstAsync();

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