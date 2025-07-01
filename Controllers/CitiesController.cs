using Microsoft.AspNetCore.Mvc;
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Operations.Interfaces;

namespace CityHotelGarageAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    public async Task<ActionResult> CreateCity(CityCreateDto cityDto)
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

    // PUT: api/Cities/5
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCity(int id, CityUpdateDto cityDto)
    {
        var result = await _cityService.UpdateCityAsync(id, cityDto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return Ok(new { message = result.Message, data = result.Data });
    }

    // DELETE: api/Cities/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCity(int id)
    {
        var result = await _cityService.DeleteCityAsync(id);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return Ok(new { message = result.Message });
    }
}