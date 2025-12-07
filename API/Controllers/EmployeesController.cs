using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _db;

    public EmployeesController(AppDbContext db)
    {
        _db = db;
    }

    // GET: api/employees
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var employees = await _db.Employees.ToListAsync();
        return Ok(employees);
    }

    // GET: api/employees/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var employee = await _db.Employees.FindAsync(id);
        if (employee == null)
            return NotFound();

        return Ok(employee);
    }

    // POST: api/employees
    [HttpPost]
    public async Task<IActionResult> Create(Employee employee)
    {
        _db.Employees.Add(employee);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
    }

    // PUT: api/employees/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Employee updated)
    {
        if (id != updated.Id)
            return BadRequest();

        var employee = await _db.Employees.FindAsync(id);
        if (employee == null)
            return NotFound();

        employee.FirstName = updated.FirstName;
        employee.LastName = updated.LastName;
        employee.Email = updated.Email;
        employee.Phone = updated.Phone;
        employee.IsActive = updated.IsActive;
        employee.CompanyId = updated.CompanyId;

        await _db.SaveChangesAsync();
        return Ok(employee);
    }

    // DELETE: api/employees/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _db.Employees.FindAsync(id);
        if (employee == null)
            return NotFound();

        _db.Employees.Remove(employee);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
