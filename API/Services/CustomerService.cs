using API.Data;
using API.Dtos.Customers;
using API.Models;
using Microsoft.EntityFrameworkCore;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _db;

    public CustomerService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        return await _db.Customers
            .Select(c => new CustomerDto
            {
                Id = c.Id,
                CompanyId = c.CompanyId,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                Phone = c.Phone
            })
            .ToListAsync();
    }

    public async Task<CustomerDto?> GetByIdAsync(int id)
    {
        return await _db.Customers
            .Where(c => c.Id == id)
            .Select(c => new CustomerDto
            {
                Id = c.Id,
                CompanyId = c.CompanyId,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                Phone = c.Phone
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerRequest dto)
    {
        var customer = new Customer
        {
            CompanyId = dto.CompanyId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone
        };

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        return await GetByIdAsync(customer.Id) ?? throw new Exception("Unexpected error");
    }

    public async Task<CustomerDto?> UpdateAsync(int id, UpdateCustomerRequest dto)
    {
        if (id != dto.Id)
            return null;

        var customer = await _db.Customers.FindAsync(id);
        if (customer == null)
            return null;

        customer.CompanyId = dto.CompanyId;
        customer.FirstName = dto.FirstName;
        customer.LastName = dto.LastName;
        customer.Email = dto.Email;
        customer.Phone = dto.Phone;

        await _db.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var customer = await _db.Customers.FindAsync(id);

        if (customer == null)
            return false;

        _db.Customers.Remove(customer);
        await _db.SaveChangesAsync();
        return true;
    }

}
