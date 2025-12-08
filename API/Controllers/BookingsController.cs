using API.Data;
using API.Dtos.Bookings;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILogger<BookingsController> _logger;

    // DbContext + Logger injiceras via konstruktor
    public BookingsController(AppDbContext db, ILogger<BookingsController> logger)
    {
        _db = db;
        _logger = logger;
    }

    // GET: api/bookings
    // Här är server-side filtrering: from, to, status
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingDetailDto>>> GetAll(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] string? status)
    {
        _logger.LogInformation(
            "GET /api/bookings called. From={From}, To={To}, Status={Status}",
            from, to, status);

        // Grundquery med include för navigation properties
        var query = _db.Bookings
            .Include(b => b.Customer)
            .Include(b => b.Employee)
            .Include(b => b.BookingServices)
                .ThenInclude(bs => bs.Service)
            .AsQueryable();

        // Filtrera på starttid från
        if (from.HasValue)
            query = query.Where(b => b.StartTime >= from.Value);

        // Filtrera på starttid till
        if (to.HasValue)
            query = query.Where(b => b.StartTime <= to.Value);

        // status är en string
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(b => b.Status == status);

        var bookings = await query
            .OrderBy(b => b.StartTime)
            .ToListAsync();

        var result = bookings.Select(MapToDetailDto).ToList();

        _logger.LogInformation(
            "GET /api/bookings returned {Count} bookings",
            result.Count);

        return Ok(result);
    }

    // GET: api/bookings/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookingDetailDto>> GetById(int id)
    {
        _logger.LogInformation("GET /api/bookings/{Id} called", id);

        var booking = await _db.Bookings
            .Include(b => b.Customer)
            .Include(b => b.Employee)
            .Include(b => b.BookingServices)
                .ThenInclude(bs => bs.Service)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
        {
            _logger.LogWarning("Booking with id {Id} not found", id);
            return NotFound();
        }

        var dto = MapToDetailDto(booking);

        _logger.LogInformation("GET /api/bookings/{Id} succeeded", id);

        return Ok(dto);
    }

    // POST: api/bookings
    [HttpPost]
    public async Task<ActionResult<BookingDetailDto>> Create(BookingCreateDto dto)
    {
        _logger.LogInformation(
            "POST /api/bookings called for CompanyId={CompanyId}, CustomerId={CustomerId}, EmployeeId={EmployeeId}",
            dto.CompanyId, dto.CustomerId, dto.EmployeeId);

        // Grundläggande validering att referenserna finns
        var company = await _db.Companies.FindAsync(dto.CompanyId);
        if (company == null)
        {
            _logger.LogWarning("Invalid CompanyId {CompanyId} in booking create", dto.CompanyId);
            return BadRequest($"Ogiltigt företag, CompanyId {dto.CompanyId} finns inte.");
        }

        var customer = await _db.Customers.FindAsync(dto.CustomerId);
        if (customer == null)
        {
            _logger.LogWarning("Invalid CustomerId {CustomerId} in booking create", dto.CustomerId);
            return BadRequest($"Ogiltig kund, CustomerId {dto.CustomerId} finns inte.");
        }

        var employee = await _db.Employees.FindAsync(dto.EmployeeId);
        if (employee == null)
        {
            _logger.LogWarning("Invalid EmployeeId {EmployeeId} in booking create", dto.EmployeeId);
            return BadRequest($"Ogiltig anställd, EmployeeId {dto.EmployeeId} finns inte.");
        }

        // Skapa själva bokningen
        var booking = new Booking
        {
            CompanyId = dto.CompanyId,
            CustomerId = dto.CustomerId,
            EmployeeId = dto.EmployeeId,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Notes = dto.Notes,
            Status = "Booked"
        };

        // Koppla services via join-tabellen BookingServices
        foreach (var serviceId in dto.ServiceIds.Distinct())
        {
            booking.BookingServices.Add(new BookingService
            {
                ServiceId = serviceId
            });
        }

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        // Ladda navigationer igen så att vi kan bygga ett komplett BookingDetailDto
        await _db.Entry(booking).Collection(b => b.BookingServices).LoadAsync();
        await _db.Entry(booking).Reference(b => b.Customer).LoadAsync();
        await _db.Entry(booking).Reference(b => b.Employee).LoadAsync();
        foreach (var bs in booking.BookingServices)
        {
            await _db.Entry(bs).Reference(x => x.Service).LoadAsync();
        }

        var result = MapToDetailDto(booking);

        _logger.LogInformation("Booking created with Id={Id}", booking.Id);

        return CreatedAtAction(nameof(GetById), new { id = booking.Id }, result);
    }

    // PUT: api/bookings/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<BookingDetailDto>> Update(int id, BookingUpdateDto dto)
    {
        _logger.LogInformation("PUT /api/bookings/{Id} called", id);

        var booking = await _db.Bookings
            .Include(b => b.BookingServices)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
        {
            _logger.LogWarning("Booking with id {Id} not found for update", id);
            return NotFound();
        }

        // Uppdatera grundvärden
        booking.StartTime = dto.StartTime;
        booking.EndTime = dto.EndTime;
        booking.Status = dto.Status;
        booking.Notes = dto.Notes;

        // Uppdatera tjänster: rensa gamla kopplingar och lägg till nya
        booking.BookingServices.Clear();

        foreach (var serviceId in dto.ServiceIds.Distinct())
        {
            booking.BookingServices.Add(new BookingService
            {
                BookingId = booking.Id,
                ServiceId = serviceId
            });
        }

        await _db.SaveChangesAsync();

        // Ladda navigationer igen för att returnera ett komplett DTO
        await _db.Entry(booking).Reference(b => b.Customer).LoadAsync();
        await _db.Entry(booking).Reference(b => b.Employee).LoadAsync();
        foreach (var bs in booking.BookingServices)
        {
            await _db.Entry(bs).Reference(x => x.Service).LoadAsync();
        }

        var result = MapToDetailDto(booking);

        _logger.LogInformation("PUT /api/bookings/{Id} succeeded", id);

        return Ok(result);
    }

    // DELETE: api/bookings/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("DELETE /api/bookings/{Id} called", id);

        var booking = await _db.Bookings.FindAsync(id);
        if (booking == null)
        {
            _logger.LogWarning("Booking with id {Id} not found for delete", id);
            return NotFound();
        }

        _db.Bookings.Remove(booking);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Booking with id {Id} deleted", id);

        return NoContent();
    }

    // --- private mapping helper ---
    // Denna metod mappar från Booking (entity) till BookingDetailDto (DTO)
    // så att API:t inte returnerar EF-entities direkt.
    private static BookingDetailDto MapToDetailDto(Booking booking)
    {
        return new BookingDetailDto
        {
            Id = booking.Id,
            CompanyId = booking.CompanyId,
            CustomerId = booking.CustomerId,
            EmployeeId = booking.EmployeeId,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            Status = booking.Status,
            Notes = booking.Notes,
            CustomerName = booking.Customer != null
                ? $"{booking.Customer.FirstName} {booking.Customer.LastName}"
                : string.Empty,
            EmployeeName = booking.Employee != null
                ? $"{booking.Employee.FirstName} {booking.Employee.LastName}"
                : string.Empty,
            Services = booking.BookingServices
                .Where(bs => bs.Service != null)
                .Select(bs => new BookingServiceItemDto
                {
                    ServiceId = bs.ServiceId,
                    Name = bs.Service!.Name,
                    DurationMinutes = bs.Service.DurationMinutes,
                    Price = bs.Service.Price
                })
                .ToList()
        };
    }
}
