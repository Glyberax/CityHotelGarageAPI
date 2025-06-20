using CityHotelGarageAPI.Operations.DTOs;
using CityHotelGarageAPI.Operations.Interfaces;
using CityHotelGarageAPI.Operations.Results;
using CityHotelGarageAPI.Repository.Interfaces;
using CityHotelGarageAPI.Repository.Models;

namespace CityHotelGarageAPI.Operations.Services;

public class CarService : ICarService
{
    private readonly ICarRepository _carRepository;
    private readonly IGarageRepository _garageRepository;

    public CarService(ICarRepository carRepository, IGarageRepository garageRepository)
    {
        _carRepository = carRepository;
        _garageRepository = garageRepository;
    }

    public async Task<ServiceResult<IEnumerable<CarDto>>> GetAllCarsAsync()
    {
        try
        {
            var cars = await _carRepository.GetCarsWithDetailsAsync();
            var carDtos = cars.Select(c => new CarDto
            {
                Id = c.Id,
                Brand = c.Brand,
                LicensePlate = c.LicensePlate,
                OwnerName = c.OwnerName,
                EntryTime = c.EntryTime,
                GarageId = c.GarageId,
                GarageName = c.Garage?.Name ?? "",
                HotelName = c.Garage?.Hotel?.Name ?? "",
                CityName = c.Garage?.Hotel?.City?.Name ?? ""
            });

            return ServiceResult<IEnumerable<CarDto>>.Success(carDtos, "Arabalar başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return ServiceResult<IEnumerable<CarDto>>.Failure($"Arabalar getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<ServiceResult<CarDto>> GetCarByIdAsync(int id)
    {
        try
        {
            var car = await _carRepository.GetCarWithDetailsAsync(id);
            if (car == null)
            {
                return ServiceResult<CarDto>.Failure("Araba bulunamadı.");
            }

            var carDto = new CarDto
            {
                Id = car.Id,
                Brand = car.Brand,
                LicensePlate = car.LicensePlate,
                OwnerName = car.OwnerName,
                EntryTime = car.EntryTime,
                GarageId = car.GarageId,
                GarageName = car.Garage?.Name ?? "",
                HotelName = car.Garage?.Hotel?.Name ?? "",
                CityName = car.Garage?.Hotel?.City?.Name ?? ""
            };

            return ServiceResult<CarDto>.Success(carDto, "Araba başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return ServiceResult<CarDto>.Failure($"Araba getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<ServiceResult<CarDto>> GetCarByLicensePlateAsync(string licensePlate)
    {
        try
        {
            var car = await _carRepository.GetCarByLicensePlateAsync(licensePlate);
            if (car == null)
            {
                return ServiceResult<CarDto>.Failure("Belirtilen plaka ile araba bulunamadı.");
            }

            var carDto = new CarDto
            {
                Id = car.Id,
                Brand = car.Brand,
                LicensePlate = car.LicensePlate,
                OwnerName = car.OwnerName,
                EntryTime = car.EntryTime,
                GarageId = car.GarageId,
                GarageName = car.Garage?.Name ?? "",
                HotelName = car.Garage?.Hotel?.Name ?? "",
                CityName = car.Garage?.Hotel?.City?.Name ?? ""
            };

            return ServiceResult<CarDto>.Success(carDto, "Araba başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return ServiceResult<CarDto>.Failure($"Araba getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<ServiceResult<CarDto>> ParkCarAsync(CarCreateDto carDto)
    {
        try
        {
            // Plaka kontrolü
            var existingCar = await _carRepository.IsLicensePlateExistsAsync(carDto.LicensePlate);
            if (existingCar)
            {
                return ServiceResult<CarDto>.Failure("Bu plaka zaten kayıtlı!");
            }

            // Garaj kapasitesi kontrolü
            var availableSpaces = await _garageRepository.GetAvailableSpacesAsync(carDto.GarageId);
            if (availableSpaces <= 0)
            {
                return ServiceResult<CarDto>.Failure("Bu garaj dolu! Başka bir garaj seçin.");
            }

            var car = new Car
            {
                Brand = carDto.Brand,
                LicensePlate = carDto.LicensePlate,
                OwnerName = carDto.OwnerName,
                GarageId = carDto.GarageId,
                EntryTime = DateTime.UtcNow
            };

            var createdCar = await _carRepository.AddAsync(car);
            
            // Detaylı bilgiyi almak için tekrar sorgula
            var carWithDetails = await _carRepository.GetCarWithDetailsAsync(createdCar.Id);

            var resultDto = new CarDto
            {
                Id = carWithDetails!.Id,
                Brand = carWithDetails.Brand,
                LicensePlate = carWithDetails.LicensePlate,
                OwnerName = carWithDetails.OwnerName,
                EntryTime = carWithDetails.EntryTime,
                GarageId = carWithDetails.GarageId,
                GarageName = carWithDetails.Garage?.Name ?? "",
                HotelName = carWithDetails.Garage?.Hotel?.Name ?? "",
                CityName = carWithDetails.Garage?.Hotel?.City?.Name ?? ""
            };

            return ServiceResult<CarDto>.Success(resultDto, "Araba başarıyla park edildi.");
        }
        catch (Exception ex)
        {
            return ServiceResult<CarDto>.Failure($"Araba park edilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<ServiceResult<CarDto>> UpdateCarAsync(int id, CarCreateDto carDto)
    {
        try
        {
            var existingCar = await _carRepository.GetByIdAsync(id);
            if (existingCar == null)
            {
                return ServiceResult<CarDto>.Failure("Güncellenecek araba bulunamadı.");
            }

            // Plaka kontrolü (kendisi hariç)
            var existingCarWithPlate = await _carRepository.GetCarByLicensePlateAsync(carDto.LicensePlate);
            if (existingCarWithPlate != null && existingCarWithPlate.Id != id)
            {
                return ServiceResult<CarDto>.Failure("Bu plaka başka bir araba tarafından kullanılıyor!");
            }

            existingCar.Brand = carDto.Brand;
            existingCar.LicensePlate = carDto.LicensePlate;
            existingCar.OwnerName = carDto.OwnerName;
            existingCar.GarageId = carDto.GarageId;

            var updatedCar = await _carRepository.UpdateAsync(existingCar);
            
            // Detaylı bilgiyi almak için tekrar sorgula
            var carWithDetails = await _carRepository.GetCarWithDetailsAsync(updatedCar.Id);

            var resultDto = new CarDto
            {
                Id = carWithDetails!.Id,
                Brand = carWithDetails.Brand,
                LicensePlate = carWithDetails.LicensePlate,
                OwnerName = carWithDetails.OwnerName,
                EntryTime = carWithDetails.EntryTime,
                GarageId = carWithDetails.GarageId,
                GarageName = carWithDetails.Garage?.Name ?? "",
                HotelName = carWithDetails.Garage?.Hotel?.Name ?? "",
                CityName = carWithDetails.Garage?.Hotel?.City?.Name ?? ""
            };

            return ServiceResult<CarDto>.Success(resultDto, "Araba başarıyla güncellendi.");
        }
        catch (Exception ex)
        {
            return ServiceResult<CarDto>.Failure($"Araba güncellenirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<ServiceResult> RemoveCarAsync(int id)
    {
        try
        {
            var exists = await _carRepository.ExistsAsync(id);
            if (!exists)
            {
                return ServiceResult.Failure("Silinecek araba bulunamadı.");
            }

            var deleted = await _carRepository.DeleteAsync(id);
            if (!deleted)
            {
                return ServiceResult.Failure("Araba silinirken hata oluştu.");
            }

            return ServiceResult.Success("Araba başarıyla park yerinden çıkarıldı.");
        }
        catch (Exception ex)
        {
            return ServiceResult.Failure($"Araba çıkarılırken hata oluştu: {ex.Message}");
        }
    }
}