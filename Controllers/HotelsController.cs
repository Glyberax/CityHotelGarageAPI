using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Operations.Interfaces;

namespace CityHotelGarage.Business.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] 
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

    // PUT: api/Hotels (Body'deki ID kullanılır)
    [HttpPut]
    public async Task<ActionResult> UpdateHotel(HotelUpdateDto hotelDto)
    {
        // Body'deki ID'yi kullan
        var result = await _hotelService.UpdateHotelAsync(hotelDto.Id, hotelDto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return Ok(new { message = result.Message, data = result.Data });
    }

    // DELETE: api/Hotels (Body'deki ID kullanılır)
    [HttpDelete]
    public async Task<ActionResult> DeleteHotel(HotelDeleteDto deleteDto)
    {
        var result = await _hotelService.DeleteHotelAsync(deleteDto.Id);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return Ok(new { message = result.Message });
    }
}