using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Operations.Interfaces;

namespace CityHotelGarageAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // ← BU SATIRI EKLE - Bearer token zorunlu yapar
public class CitiesController : ControllerBase
{
    private readonly ICityService _cityService;

    public CitiesController(ICityService cityService)
    {
        _cityService = cityService;
    }

    // GET: api/Cities
    [HttpGet]
    public async Task<ActionResult> GetCities()
    {
        var result = await _cityService.GetAllCitiesAsync();
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return Ok(new { message = result.Message, data = result.Data });
    }

    // GET: api/Cities/5
    [HttpGet("{id}")]
    public async Task<ActionResult> GetCity(int id)
    {
        var result = await _cityService.GetCityByIdAsync(id);
        
        if (!result.IsSuccess)
        {
            return NotFound(new { message = result.Message, errors = result.Errors });
        }

        return Ok(new { message = result.Message, data = result.Data });
    }

    // POST: api/Cities
    [HttpPost]
    public async Task<ActionResult> CreateCity(CityCreateDto cityDto) // ✅ Method ismi tutarlı
    {
        var result = await _cityService.CreateCityAsync(cityDto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return CreatedAtAction(nameof(GetCity), 
            new { id = result.Data!.Id }, 
            new { message = result.Message, data = result.Data });
    }

    // PUT: api/Cities (Body'deki ID kullanılır)
    [HttpPut]
    public async Task<ActionResult> UpdateCity(CityUpdateDto cityDto) // ✅ CityUpdateDto + URL'den ID kaldırıldı
    {
        // Body'deki ID'yi kullan
        var result = await _cityService.UpdateCityAsync(cityDto.Id, cityDto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return Ok(new { message = result.Message, data = result.Data });
    }

    // DELETE: api/Cities (Body'deki ID kullanılır)
    [HttpDelete]
    public async Task<ActionResult> DeleteCity(CityDeleteDto deleteDto)
    {
        var result = await _cityService.DeleteCityAsync(deleteDto.Id);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return Ok(new { message = result.Message });
    }
}