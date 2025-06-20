using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CityHotelGarageAPI;

[ApiController]
[Route("api/[controller]")]
public class CarsController : ControllerBase
{
    private readonly AppDbContext _context;

    public CarsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Cars
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetCars()
    {
        var cars = await _context.Cars
            .Include(c => c.Garage)
            .ThenInclude(g => g.Hotel)
            .ThenInclude(h => h.City)
            .Select(c => new
            {
                c.Id,
                c.Brand,
                c.LicensePlate,
                c.OwnerName,
                c.EntryTime,
                GarageName = c.Garage.Name,
                HotelName = c.Garage.Hotel.Name,
                CityName = c.Garage.Hotel.City.Name,
                c.GarageId
            })
            .ToListAsync();

        return Ok(cars);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetCar(int id)
    {
        var car = await _context.Cars
            .Include(c => c.Garage)
            .ThenInclude(g => g.Hotel)
            .ThenInclude(h => h.City)
            .Where(c => c.Id == id)
            .Select(c => new
            {
                c.Id,
                c.Brand,
                c.LicensePlate,
                c.OwnerName,
                c.EntryTime,
                Garage = new 
                { 
                    c.Garage.Id, 
                    c.Garage.Name,
                    Hotel = new { c.Garage.Hotel.Id, c.Garage.Hotel.Name },
                    City = new { c.Garage.Hotel.City.Id, c.Garage.Hotel.City.Name }
                }
            })
            .FirstOrDefaultAsync();

        if (car == null)
        {
            return NotFound();
        }

        return Ok(car);
    }

    // GET: api/Cars/ByLicensePlate/{licensePlate}
    [HttpGet("ByLicensePlate/{licensePlate}")]
    public async Task<ActionResult<object>> GetCarByLicensePlate(string licensePlate)
    {
        var car = await _context.Cars
            .Include(c => c.Garage)
            .ThenInclude(g => g.Hotel)
            .ThenInclude(h => h.City)
            .Where(c => c.LicensePlate == licensePlate)
            .Select(c => new
            {
                c.Id,
                c.Brand,
                c.LicensePlate,
                c.OwnerName,
                c.EntryTime,
                Garage = new 
                { 
                    c.Garage.Id, 
                    c.Garage.Name,
                    Hotel = new { c.Garage.Hotel.Id, c.Garage.Hotel.Name },
                    City = new { c.Garage.Hotel.City.Id, c.Garage.Hotel.City.Name }
                }
            })
            .FirstOrDefaultAsync();

        if (car == null)
        {
            return NotFound("Belirtilen plaka ile araba bulunamadı.");
        }

        return Ok(car);
    }

    // POST: api/Cars (Yeni araba park et)
    [HttpPost]
    public async Task<ActionResult<object>> PostCar(Car car)
    {
        var garage = await _context.Garages
            .Include(g => g.Cars)
            .FirstOrDefaultAsync(g => g.Id == car.GarageId);

        if (garage == null)
        {
            return BadRequest("Belirtilen garaj bulunamadı.");
        }

        if (garage.Cars.Count >= garage.Capacity)
        {
            return BadRequest("Bu garaj dolu! Başka bir garaj seçin.");
        }

        // Plaka kontrolü
        var existingCar = await _context.Cars
            .FirstOrDefaultAsync(c => c.LicensePlate == car.LicensePlate);
        
        if (existingCar != null)
        {
            return BadRequest("Bu plaka zaten kayıtlı!");
        }

        car.EntryTime = DateTime.UtcNow;
        _context.Cars.Add(car);
        await _context.SaveChangesAsync();

        var addedCar = await _context.Cars
            .Include(c => c.Garage)
            .ThenInclude(g => g.Hotel)
            .ThenInclude(h => h.City)
            .Where(c => c.Id == car.Id)
            .Select(c => new
            {
                c.Id,
                c.Brand,
                c.LicensePlate,
                c.OwnerName,
                c.EntryTime,
                Garage = new 
                { 
                    c.Garage.Id, 
                    c.Garage.Name,
                    Hotel = new { c.Garage.Hotel.Id, c.Garage.Hotel.Name },
                    City = new { c.Garage.Hotel.City.Id, c.Garage.Hotel.City.Name }
                }
            })
            .FirstOrDefaultAsync();

        return CreatedAtAction("GetCar", new { id = car.Id }, addedCar);
    }

    // PUT: api/Cars/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCar(int id, Car car)
    {
        if (id != car.Id)
        {
            return BadRequest();
        }

        // Garaj var mı kontrol et
        var garageExists = await _context.Garages.AnyAsync(g => g.Id == car.GarageId);
        if (!garageExists)
        {
            return BadRequest("Belirtilen garaj bulunamadı.");
        }

        // Plaka kontrolü (kendisi hariç)
        var existingCar = await _context.Cars
            .FirstOrDefaultAsync(c => c.LicensePlate == car.LicensePlate && c.Id != id);
        
        if (existingCar != null)
        {
            return BadRequest("Bu plaka başka bir araba tarafından kullanılıyor!");
        }

        _context.Entry(car).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CarExists(id))
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

    // DELETE: api/Cars/5 (Arabayı park yerinden çıkar)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCar(int id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car == null)
        {
            return NotFound();
        }

        _context.Cars.Remove(car);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CarExists(int id)
    {
        return _context.Cars.Any(e => e.Id == id);
    }
}