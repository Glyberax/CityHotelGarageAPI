using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CityHotelGarageAPI;

[ApiController]
[Route("api/[controller]")]
public class CitiesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CitiesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Cities
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetCities()
    {
        var cities = await _context.Cities
            .Include(c => c.Hotels)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Population,
                c.CreatedDate,
                HotelCount = c.Hotels.Count()
            })
            .ToListAsync();

        return Ok(cities);
    }

    // GET: api/Cities/5
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetCity(int id)
    {
        var city = await _context.Cities
            .Include(c => c.Hotels)
            .Where(c => c.Id == id)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Population,
                c.CreatedDate,
                Hotels = c.Hotels.Select(h => new
                {
                    h.Id,
                    h.Name,
                    h.Yildiz
                })
            })
            .FirstOrDefaultAsync();

        if (city == null)
        {
            return NotFound();
        }

        return Ok(city);
    }

    // POST: api/Cities
    [HttpPost]
    public async Task<ActionResult<City>> PostCity(City city)
    {
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetCity", new { id = city.Id }, city);
    }

    // PUT: api/Cities/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCity(int id, City city)
    {
        if (id != city.Id)
        {
            return BadRequest();
        }

        _context.Entry(city).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CityExists(id))
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

    // DELETE: api/Cities/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCity(int id)
    {
        var city = await _context.Cities.FindAsync(id);
        if (city == null)
        {
            return NotFound();
        }

        _context.Cities.Remove(city);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CityExists(int id)
    {
        return _context.Cities.Any(e => e.Id == id);
    }
}