using CityHotelGarageAPI.Operations.DTOs;
using CityHotelGarageAPI.Repository.Models;
using System.Linq.Expressions;

namespace CityHotelGarageAPI.Operations.Extensions;

public static class MappingExtensions
{
    // Car Entity'sini CarDto'ya dönüştürmek için Expression
    // Bu expression database tarafında çalışır (SQL'e çevrilir)
    public static Expression<Func<Car, CarDto>> ToCarDto =>
        c => new CarDto
        {
            Id = c.Id,
            Brand = c.Brand,
            LicensePlate = c.LicensePlate,
            OwnerName = c.OwnerName,
            EntryTime = c.EntryTime,
            GarageId = c.GarageId,
            GarageName = c.Garage != null ? c.Garage.Name : "",
            HotelName = c.Garage != null && c.Garage.Hotel != null ? c.Garage.Hotel.Name : "",
            CityName = c.Garage != null && c.Garage.Hotel != null && c.Garage.Hotel.City != null ? c.Garage.Hotel.City.Name : ""
        };

    // City Entity'sini CityDto'ya dönüştürmek için Expression
    public static Expression<Func<City, CityDto>> ToCityDto =>
        c => new CityDto
        {
            Id = c.Id,
            Name = c.Name,
            Population = c.Population,
            CreatedDate = c.CreatedDate,
            HotelCount = c.Hotels.Count()
        };

    // Hotel Entity'sini HotelDto'ya dönüştürmek için Expression
    public static Expression<Func<Hotel, HotelDto>> ToHotelDto =>
        h => new HotelDto
        {
            Id = h.Id,
            Name = h.Name,
            Yildiz = h.Yildiz,
            CreatedDate = h.CreatedDate,
            CityId = h.CityId,
            CityName = h.City != null ? h.City.Name : "",
            GarageCount = h.Garages.Count()
        };

    // Garage Entity'sini GarageDto'ya dönüştürmek için Expression
    public static Expression<Func<Garage, GarageDto>> ToGarageDto =>
        g => new GarageDto
        {
            Id = g.Id,
            Name = g.Name,
            Capacity = g.Capacity,
            CreatedDate = g.CreatedDate,
            HotelId = g.HotelId,
            HotelName = g.Hotel != null ? g.Hotel.Name : "",
            CityName = g.Hotel != null && g.Hotel.City != null ? g.Hotel.City.Name : "",
            CarCount = g.Cars.Count(),
            AvailableSpaces = g.Capacity - g.Cars.Count()
        };

    // IQueryable extension metodları - bunlar IQueryable'ı DTO'ya dönüştürür
    
    // Kullanım: _carRepository.GetCarsWithDetails().ProjectToCarDto()
    public static IQueryable<CarDto> ProjectToCarDto(this IQueryable<Car> query)
    {
        return query.Select(ToCarDto);
    }

    // Kullanım: _cityRepository.GetAll().ProjectToCityDto()
    public static IQueryable<CityDto> ProjectToCityDto(this IQueryable<City> query)
    {
        return query.Select(ToCityDto);
    }

    // Kullanım: _hotelRepository.GetAll().ProjectToHotelDto()
    public static IQueryable<HotelDto> ProjectToHotelDto(this IQueryable<Hotel> query)
    {
        return query.Select(ToHotelDto);
    }

    // Kullanım: _garageRepository.GetAll().ProjectToGarageDto()
    public static IQueryable<GarageDto> ProjectToGarageDto(this IQueryable<Garage> query)
    {
        return query.Select(ToGarageDto);
    }
}