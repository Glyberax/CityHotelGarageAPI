using Microsoft.AspNetCore.Mvc;
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Operations.Interfaces;

namespace CityHotelGarageAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GaragesController : ControllerBase
{
    private readonly IGarageService _garageService;

    public GaragesController(IGarageService garageService)
    {
        _garageService = garageService;
    }

    // GET: api/Garages
    [HttpGet]
    public async Task<ActionResult> GetGarages()
    {
        var result = await _garageService.GetAllGaragesAsync();
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return Ok(new { message = result.Message, data = result.Data });
    }

    // GET: api/Garages/5
    [HttpGet("{id}")]
    public async Task<ActionResult> GetGarage(int id)
    {
        var result = await _garageService.GetGarageByIdAsync(id);
        
        if (!result.IsSuccess)
        {
            return NotFound(new { message = result.Message, errors = result.Errors });
        }

        return Ok(new { message = result.Message, data = result.Data });
    }

    // GET: api/Garages/ByHotel/{hotelId}
    [HttpGet("ByHotel/{hotelId}")]
    public async Task<ActionResult> GetGaragesByHotel(int hotelId)
    {
        var result = await _garageService.GetGaragesByHotelAsync(hotelId);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return Ok(new { message = result.Message, data = result.Data });
    }

    // POST: api/Garages
    [HttpPost]
    public async Task<ActionResult> CreateGarage(GarageCreateDto garageDto)
    {
        var result = await _garageService.CreateGarageAsync(garageDto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return CreatedAtAction(nameof(GetGarage), 
            new { id = result.Data!.Id }, 
            new { message = result.Message, data = result.Data });
    }

    // PUT: api/Garages (Body'deki ID kullanılır)
    [HttpPut]
    public async Task<ActionResult> UpdateGarage(GarageUpdateDto garageDto)
    {
        // Body'deki ID'yi kullan
        var result = await _garageService.UpdateGarageAsync(garageDto.Id, garageDto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return Ok(new { message = result.Message, data = result.Data });
    }

    // DELETE: api/Garages (Body'deki ID kullanılır)
    [HttpDelete]
    public async Task<ActionResult> DeleteGarage(GarageDeleteDto deleteDto)
    {
        var result = await _garageService.DeleteGarageAsync(deleteDto.Id);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return Ok(new { message = result.Message });
    }
}