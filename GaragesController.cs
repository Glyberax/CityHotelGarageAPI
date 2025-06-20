using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CityHotelGarageAPI;

[ApiController]
[Route("api/[controller]")]
public class GaragesController : ControllerBase
{
    private readonly AppDbContext _context;

    public GaragesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Garages
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetGarages()
    {
        var garages = await _context.Garages
            .Include(g => g.Hotel)
            .ThenInclude(h => h.City)
            .Include(g => g.Cars)
            .Select(g => new
            {
                g.Id,
                g.Name,
                g.Capacity,
                g.CreatedDate,
                HotelName = g.Hotel.Name,
                CityName = g.Hotel.City.Name,
                g.HotelId,
                CarCount = g.Cars.Count(),
                AvailableSpaces = g.Capacity - g.Cars.Count()
            })
            .ToListAsync();

        return Ok(garages);
    }

    // GET: api/Garages/5
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetGarage(int id)
    {
        var garage = await _context.Garages
            .Include(g => g.Hotel)
            .ThenInclude(h => h.City)
            .Include(g => g.Cars)
            .Where(g => g.Id == id)
            .Select(g => new
            {
                g.Id,
                g.Name,
                g.Capacity,
                g.CreatedDate,
                Hotel = new { g.Hotel.Id, g.Hotel.Name },
                City = new { g.Hotel.City.Id, g.Hotel.City.Name },
                Cars = g.Cars.Select(c => new
                {
                    c.Id,
                    c.Brand,
                    c.LicensePlate,
                    c.OwnerName,
                    c.EntryTime
                }),
                CarCount = g.Cars.Count(),
                AvailableSpaces = g.Capacity - g.Cars.Count()
            })
            .FirstOrDefaultAsync();

        if (garage == null)
        {
            return NotFound();
        }

        return Ok(garage);
    }

    // POST: api/Garages
    [HttpPost]
    public async Task<ActionResult<Garage>> PostGarage(Garage garage)
    {
        // Otel var mı kontrol et
        var hotelExists = await _context.Hotels.AnyAsync(h => h.Id == garage.HotelId);
        if (!hotelExists)
        {
            return BadRequest("Belirtilen otel bulunamadı.");
        }

        _context.Garages.Add(garage);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetGarage", new { id = garage.Id }, garage);
    }

    // PUT: api/Garages/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutGarage(int id, Garage garage)
    {
        if (id != garage.Id)
        {
            return BadRequest();
        }

        // Otel var mı kontrol et
        var hotelExists = await _context.Hotels.AnyAsync(h => h.Id == garage.HotelId);
        if (!hotelExists)
        {
            return BadRequest("Belirtilen otel bulunamadı.");
        }

        _context.Entry(garage).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!GarageExists(id))
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

    // DELETE: api/Garages/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGarage(int id)
    {
        var garage = await _context.Garages.FindAsync(id);
        if (garage == null)
        {
            return NotFound();
        }

        _context.Garages.Remove(garage);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool GarageExists(int id)
    {
        return _context.Garages.Any(e => e.Id == id);
    }
}