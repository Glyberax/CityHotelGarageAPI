using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CityHotelGarage.Business.Repository.Data;
using CityHotelGarage.Business.Repository.Models;

namespace CityHotelGarage.Business.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelsController : ControllerBase
{
    private readonly AppDbContext _context;

    public HotelsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Hotels
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetHotels()
    {
        var hotels = await _context.Hotels
            .Include(h => h.City)
            .Include(h => h.Garages)
            .Select(h => new
            {
                h.Id,
                h.Name,
                h.Yildiz,
                h.CreatedDate,
                CityName = h.City.Name,
                h.CityId,
                GarageCount = h.Garages.Count()
            })
            .ToListAsync();

        return Ok(hotels);
    }

    // GET: api/Hotels/5
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetHotel(int id)
    {
        var hotel = await _context.Hotels
            .Include(h => h.City)
            .Include(h => h.Garages)
            .Where(h => h.Id == id)
            .Select(h => new
            {
                h.Id,
                h.Name,
                h.Yildiz,
                h.CreatedDate,
                City = new { h.City.Id, h.City.Name },
                Garages = h.Garages.Select(g => new
                {
                    g.Id,
                    g.Name,
                    g.Capacity
                })
            })
            .FirstOrDefaultAsync();

        if (hotel == null)
        {
            return NotFound();
        }

        return Ok(hotel);
    }

    // POST: api/Hotels
    [HttpPost]
    public async Task<ActionResult<Hotel>> PostHotel(Hotel hotel)
    {
        // Şehir var mı kontrol et
        var cityExists = await _context.Cities.AnyAsync(c => c.Id == hotel.CityId);
        if (!cityExists)
        {
            return BadRequest("Belirtilen şehir bulunamadı.");
        }

        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);
    }

    // PUT: api/Hotels/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutHotel(int id, Hotel hotel)
    {
        if (id != hotel.Id)
        {
            return BadRequest();
        }

        // Şehir var mı kontrol
        var cityExists = await _context.Cities.AnyAsync(c => c.Id == hotel.CityId);
        if (!cityExists)
        {
            return BadRequest("Belirtilen şehir bulunamadı.");
        }

        _context.Entry(hotel).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!HotelExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/Hotels/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel == null)
        {
            return NotFound();
        }

        _context.Hotels.Remove(hotel);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool HotelExists(int id)
    {
        return _context.Hotels.Any(e => e.Id == id);
    }
}