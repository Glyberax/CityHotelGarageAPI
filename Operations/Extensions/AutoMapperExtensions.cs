using AutoMapper;
using AutoMapper.QueryableExtensions;
using CityHotelGarageAPI.Operations.DTOs;
using CityHotelGarageAPI.Repository.Models;

namespace CityHotelGarageAPI.Operations.Extensions;

public static class AutoMapperExtensions
{
    // IQueryable projection extension methods using AutoMapper
    // Fully qualified AutoMapper.IConfigurationProvider kullanarak namespace conflict'i çözüyoruz
    
    public static IQueryable<CarDto> ProjectToCarDto(this IQueryable<Car> query, AutoMapper.IConfigurationProvider configuration)
    {
        return query.ProjectTo<CarDto>(configuration);
    }

    public static IQueryable<CityDto> ProjectToCityDto(this IQueryable<City> query, AutoMapper.IConfigurationProvider configuration)
    {
        return query.ProjectTo<CityDto>(configuration);
    }

    public static IQueryable<HotelDto> ProjectToHotelDto(this IQueryable<Hotel> query, AutoMapper.IConfigurationProvider configuration)
    {
        return query.ProjectTo<HotelDto>(configuration);
    }

    public static IQueryable<GarageDto> ProjectToGarageDto(this IQueryable<Garage> query, AutoMapper.IConfigurationProvider configuration)
    {
        return query.ProjectTo<GarageDto>(configuration);
    }
}