using Microsoft.AspNetCore.Mvc;
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Operations.Interfaces;

namespace CityHotelGarageAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelsController : ControllerBase
{
    private readonly IHotelService _hotelService;

    public HotelsController(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    // GET: api/Hotels
    [HttpGet]
    public async Task<ActionResult> GetHotels()
    {
        var result = await _hotelService.GetAllHotelsAsync();
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return Ok(new { message = result.Message, data = result.Data });
    }

    // GET: api/Hotels/5
    [HttpGet("{id}")]
    public async Task<ActionResult> GetHotel(int id)
    {
        var result = await _hotelService.GetHotelByIdAsync(id);
        
        if (!result.IsSuccess)
        {
            return NotFound(new { message = result.Message, errors = result.Errors });
        }

        return Ok(new { message = result.Message, data = result.Data });
    }

    // GET: api/Hotels/ByCity/{cityId}
    [HttpGet("ByCity/{cityId}")]
    public async Task<ActionResult> GetHotelsByCity(int cityId)
    {
        var result = await _hotelService.GetHotelsByCityAsync(cityId);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return Ok(new { message = result.Message, data = result.Data });
    }

    // POST: api/Hotels
    [HttpPost]
    public async Task<ActionResult> CreateHotel(HotelCreateDto hotelDto)
    {
        var result = await _hotelService.CreateHotelAsync(hotelDto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return CreatedAtAction(nameof(GetHotel), 
            new { id = result.Data!.Id }, 
            new { message = result.Message, data = result.Data });
    }

    // PUT: api/Hotels/5
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateHotel(int id, HotelUpdateDto hotelDto)
    {
        var result = await _hotelService.UpdateHotelAsync(id, hotelDto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return Ok(new { message = result.Message, data = result.Data });
    }

    // DELETE: api/Hotels/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteHotel(int id)
    {
        var result = await _hotelService.DeleteHotelAsync(id);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return Ok(new { message = result.Message });
    }
}