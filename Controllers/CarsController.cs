using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Operations.Interfaces;

namespace CityHotelGarageAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // ← BU SATIRI EKLE - Bearer token zorunlu yapar
public class CarsController : ControllerBase
{
    private readonly ICarService _carService;

    public CarsController(ICarService carService)
    {
        _carService = carService;
    }

    // GET: api/Cars
    [HttpGet]
    public async Task<ActionResult> GetCars()
    {
        var result = await _carService.GetAllCarsAsync();
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return Ok(new { message = result.Message, data = result.Data });
    }

    // GET: api/Cars/5
    [HttpGet("{id}")]
    public async Task<ActionResult> GetCar(int id)
    {
        var result = await _carService.GetCarByIdAsync(id);
        
        if (!result.IsSuccess)
        {
            return NotFound(new { message = result.Message, errors = result.Errors });
        }

        return Ok(new { message = result.Message, data = result.Data });
    }

    // GET: api/Cars/ByLicensePlate/{licensePlate}
    [HttpGet("ByLicensePlate/{licensePlate}")]
    public async Task<ActionResult> GetCarByLicensePlate(string licensePlate)
    {
        var result = await _carService.GetCarByLicensePlateAsync(licensePlate);
        
        if (!result.IsSuccess)
        {
            return NotFound(new { message = result.Message, errors = result.Errors });
        }

        return Ok(new { message = result.Message, data = result.Data });
    }

    // GET: api/Cars/ByGarage/{garageId}
    [HttpGet("ByGarage/{garageId}")]
    public async Task<ActionResult> GetCarsByGarage(int garageId)
    {
        var result = await _carService.GetCarsByGarageAsync(garageId);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return Ok(new { message = result.Message, data = result.Data });
    }

    // POST: api/Cars (Yeni araba park et)
    [HttpPost]
    public async Task<ActionResult> CreateCar(CarCreateDto carDto) // ✅ Method ismi tutarlı
    {
        var result = await _carService.CreateCarAsync(carDto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return CreatedAtAction(nameof(GetCar), 
            new { id = result.Data!.Id }, 
            new { message = result.Message, data = result.Data });
    }

    // PUT: api/Cars (Body'deki ID kullanılır)
    [HttpPut]
    public async Task<ActionResult> UpdateCar(CarUpdateDto carDto)
    {
        // Body'deki ID'yi kullan
        var result = await _carService.UpdateCarAsync(carDto.Id, carDto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return Ok(new { message = result.Message, data = result.Data });
    }

    // DELETE: api/Cars (Body'deki ID kullanılır) 
    [HttpDelete]
    public async Task<ActionResult> DeleteCar(CarDeleteDto deleteDto)
    {
        var result = await _carService.DeleteCarAsync(deleteDto.Id);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return Ok(new { message = result.Message });
    }
}