using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Operations.Interfaces;

namespace CityHotelGarageAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] 
public class CitiesController : ControllerBase
{
    private readonly ICityService _cityService;

    public CitiesController(ICityService cityService)
    {
        _cityService = cityService;
    }

    // GET: api/Cities - Tüm şehirler (eski endpoint)
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


    /// <summary>
    /// Sayfalı şehir listesi - Arama, sıralama ve filtreleme destekli
    /// </summary>
    /// <param name="pagingRequest">Sayfalama parametreleri</param>
    /// <returns>Sayfalı şehir listesi</returns>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResult<CityDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> GetPagedCities([FromQuery] PagingRequestDto pagingRequest)
    {
        var result = await _cityService.GetPagedCitiesAsync(pagingRequest);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { 
                message = result.Message, 
                errors = result.Errors 
            });
        }

        return Ok(result.Data);
    }
    
    /// <param name="searchTerm">Arama terimi</param>
    /// <param name="limit">Maksimum sonuç sayısı (default: 10)</param>
    /// <returns>Arama sonuçları</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResult<CityDto>), 200)]
    public async Task<ActionResult> SearchCities([FromQuery] string searchTerm, [FromQuery] int limit = 10)
    {
        var pagingRequest = new PagingRequestDto
        {
            PageNumber = 1,
            PageSize = Math.Min(limit, 50), // Max 50 sonuç
            SearchTerm = searchTerm,
            SortBy = "name",
            SortDescending = false
        };

        var result = await _cityService.GetPagedCitiesAsync(pagingRequest);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message });
        }

        return Ok(result.Data);
    }

    // GET: api/Cities/{id} - Tekil şehir
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

    // POST: api/Cities - Yeni şehir oluştur
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

    // PUT: api/Cities - Şehir güncelle
    [HttpPut]
    public async Task<ActionResult> UpdateCity(CityUpdateDto cityDto)
    {
        var result = await _cityService.UpdateCityAsync(cityDto.Id, cityDto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return Ok(new { message = result.Message, data = result.Data });
    }

    // DELETE: api/Cities - Şehir sil
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