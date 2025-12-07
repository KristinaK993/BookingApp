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

    public BookingsController(AppDbContext db)
    {
        _db = db;
    }

    // GET: api/bookings
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingDetailDto>>> GetAll(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] string? status)
    {
        var query = _db.Bookings
            .Include(b => b.Customer)
            .Include(b => b.Employee)
            .Include(b => b.BookingServices)
                .ThenInclude(bs => bs.Service)
            .AsQueryable();

        if (from.HasValue)
            query = query.Where(b => b.StartTime >= from.Value);

        if (to.HasValue)
            query = query.Where(b => b.StartTime <= to.Value);

        // status är nu string, inte enum
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(b => b.Status == status);

        var bookings = await query
            .OrderBy(b => b.StartTime)
            .ToListAsync();

        var result = bookings.Select(MapToDetailDto).ToList();

        return Ok(result);
    }

    // GET: api/bookings/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookingDetailDto>> GetById(int id)
    {
        var booking = await _db.Bookings
            .Include(b => b.Customer)
            .Include(b => b.Employee)
            .Include(b => b.BookingServices)
                .ThenInclude(bs => bs.Service)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
            return NotFound();

        return Ok(MapToDetailDto(booking));
    }

    // POST: api/bookings
    [HttpPost]
    public async Task<ActionResult<BookingDetailDto>> Create(BookingCreateDto dto)
    {
        // Grundläggande koll – finns referenserna?
        var company = await _db.Companies.FindAsync(dto.CompanyId);
        if (company == null)
            return BadRequest($"Ogiltigt företag, CompanyId {dto.CompanyId} finns inte.");

        var customer = await _db.Customers.FindAsync(dto.CustomerId);
        if (customer == null)
            return BadRequest($"Ogiltig kund, CustomerId {dto.CustomerId} finns inte.");

        var employee = await _db.Employees.FindAsync(dto.EmployeeId);
        if (employee == null)
            return BadRequest($"Ogiltig anställd, EmployeeId {dto.EmployeeId} finns inte.");

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

        // Koppla services
        foreach (var serviceId in dto.ServiceIds.Distinct())
        {
            booking.BookingServices.Add(new BookingService
            {
                ServiceId = serviceId
            });
        }

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        // Ladda om inklusive navigationer
        await _db.Entry(booking).Collection(b => b.BookingServices).LoadAsync();
        await _db.Entry(booking).Reference(b => b.Customer).LoadAsync();
        await _db.Entry(booking).Reference(b => b.Employee).LoadAsync();
        foreach (var bs in booking.BookingServices)
        {
            await _db.Entry(bs).Reference(x => x.Service).LoadAsync();
        }

        var result = MapToDetailDto(booking);

        return CreatedAtAction(nameof(GetById), new { id = booking.Id }, result);
    }

    // PUT: api/bookings/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<BookingDetailDto>> Update(int id, BookingUpdateDto dto)
    {
        var booking = await _db.Bookings
            .Include(b => b.BookingServices)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
            return NotFound();

        booking.StartTime = dto.StartTime;
        booking.EndTime = dto.EndTime;
        booking.Status = dto.Status;
        booking.Notes = dto.Notes;

        // uppdatera services: ta bort gamla, lägg till nya
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

        // Ladda om för att kunna returnera komplett DTO
        await _db.Entry(booking).Reference(b => b.Customer).LoadAsync();
        await _db.Entry(booking).Reference(b => b.Employee).LoadAsync();
        foreach (var bs in booking.BookingServices)
        {
            await _db.Entry(bs).Reference(x => x.Service).LoadAsync();
        }

        return Ok(MapToDetailDto(booking));
    }

    // DELETE: api/bookings/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var booking = await _db.Bookings.FindAsync(id);
        if (booking == null)
            return NotFound();

        _db.Bookings.Remove(booking);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // --- private mapping helper ---

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
