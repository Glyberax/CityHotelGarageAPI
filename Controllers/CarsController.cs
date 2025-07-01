using Microsoft.AspNetCore.Mvc;
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Operations.Interfaces;

namespace CityHotelGarageAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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

    // POST: api/Cars (Yeni araba park et)
    [HttpPost]
    public async Task<ActionResult> PostCar(CarCreateDto carDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _carService.ParkCarAsync(carDto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return CreatedAtAction(nameof(GetCar), 
            new { id = result.Data!.Id }, 
            new { message = result.Message, data = result.Data });
    }

    // PUT: api/Cars/5
    [HttpPut("[action]/{id}")]
    public async Task<ActionResult> PutCar(int id, CarCreateDto carDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _carService.UpdateCarAsync(id, carDto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return Ok(new { message = result.Message, data = result.Data });
    }

    // DELETE: api/Cars/5 (Arabayı park yerinden çıkar)
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCar(int id)
    {
        var result = await _carService.RemoveCarAsync(id);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return Ok(new { message = result.Message });
    }
}// TODO: Add search functionality
