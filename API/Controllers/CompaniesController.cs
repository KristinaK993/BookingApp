using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly AppDbContext _db;

    public CompaniesController(AppDbContext db)
    {
        _db = db;
    }

    // GET: api/companies
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Company>>> GetAll()
    {
        var companies = await _db.Companies.ToListAsync();
        return Ok(companies);
    }

    // GET: api/companies/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Company>> GetById(int id)
    {
        var company = await _db.Companies.FindAsync(id);
        if (company == null)
            return NotFound();

        return Ok(company);
    }

    // POST: api/companies
    [HttpPost]
    public async Task<ActionResult<Company>> Create(Company company)
    {
        _db.Companies.Add(company);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = company.Id }, company);
    }

    // PUT: api/companies/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<Company>> Update(int id, Company updated)
    {
        if (id != updated.Id)
            return BadRequest();

        var company = await _db.Companies.FindAsync(id);
        if (company == null)
            return NotFound();

        company.Name = updated.Name;
        company.Address = updated.Address;
        company.Phone = updated.Phone;
        company.Email = updated.Email;

        await _db.SaveChangesAsync();
        return Ok(company);
    }

    // DELETE: api/companies/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var company = await _db.Companies.FindAsync(id);
        if (company == null)
            return NotFound();

        _db.Companies.Remove(company);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
