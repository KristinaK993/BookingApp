using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly AppDbContext _db;

    public ServicesController(AppDbContext db)
    {
        _db = db;
    }

    // GET: api/services
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var services = await _db.Services.ToListAsync();
        return Ok(services);
    }

    // GET: api/services/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var service = await _db.Services.FindAsync(id);
        if (service == null)
            return NotFound();

        return Ok(service);
    }

    // POST: api/services
    [HttpPost]
    public async Task<IActionResult> Create(Service service)
    {
        _db.Services.Add(service);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = service.Id }, service);
    }

    // PUT: api/services/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Service updated)
    {
        if (id != updated.Id)
            return BadRequest();

        var service = await _db.Services.FindAsync(id);
        if (service == null)
            return NotFound();

        service.Name = updated.Name;
        service.Description = updated.Description;
        service.DurationMinutes = updated.DurationMinutes;
        service.Price = updated.Price;
        service.CompanyId = updated.CompanyId;

        await _db.SaveChangesAsync();
        return Ok(service);
    }

    // DELETE: api/services/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var service = await _db.Services.FindAsync(id);
        if (service == null)
            return NotFound();

        _db.Services.Remove(service);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
