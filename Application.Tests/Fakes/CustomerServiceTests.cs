using API.Data;
using API.Models;
using API.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using API.Dtos.Customers;


namespace Application.Tests;

public class CustomerServiceTests
{
    private AppDbContext _db = null!;
    private CustomerService _service = null!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);
        _service = new CustomerService(_db);
    }

    [TearDown]
    public void TearDown()
    {
        _db.Dispose(); 
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnNull_WhenCustomerDoesNotExist()
    {
        var result = await _service.GetByIdAsync(999);
        Assert.IsNull(result);
    }

    [Test]
    public async Task DeleteAsync_ShouldReturnTrue_WhenCustomerExists()
    {
        // ARRANGE – lägg till en kund i den fejkade InMemory-databasen
        var customer = new Customer
        {
            CompanyId = 1,           // enkel dummy-data
            FirstName = "Test",
            LastName = "Customer",
            Email = "test@example.com",
            Phone = "123456789"
        };

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        // Kontroll: kunden har fått ett Id
        Assert.That(customer.Id, Is.GreaterThan(0));

        // ACT – försök ta bort kunden via servicen
        var result = await _service.DeleteAsync(customer.Id);

        // ASSERT – metoden ska returnera true och kunden ska vara borta
        Assert.IsTrue(result);

        var deleted = await _db.Customers.FindAsync(customer.Id);
        Assert.IsNull(deleted);
    }

    [Test]
    public async Task CreateAsync_ShouldAddCustomer()
    {
        // ARRANGE
        var dto = new CreateCustomerRequest
        {
            CompanyId = 1,
            FirstName = "Anna",
            LastName = "Svensson",
            Email = "anna@example.com",
            Phone = "123456"
        };

        // ACT
        var created = await _service.CreateAsync(dto);

        // ASSERT
        Assert.IsNotNull(created);
        Assert.That(created.Id, Is.GreaterThan(0));
        Assert.That(created.FirstName, Is.EqualTo("Anna"));

        // Kontrollera i databasen
        var dbCustomer = await _db.Customers.FindAsync(created.Id);
        Assert.IsNotNull(dbCustomer);
        Assert.That(dbCustomer!.Email, Is.EqualTo("anna@example.com"));
    }

    [Test]
    public async Task UpdateAsync_ShouldUpdateCustomer()
    {
        // ARRANGE – skapa kund i databasen först
        var customer = new Customer
        {
            CompanyId = 1,
            FirstName = "Anna",
            LastName = "Svensson",
            Email = "anna@example.com",
            Phone = "123456"
        };

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        // skapa DTO för uppdatering
        var dto = new UpdateCustomerRequest
        {
            Id = customer.Id,
            CompanyId = customer.CompanyId,
            FirstName = "Anna-Maria",   // ändrat namn
            LastName = "Svensson",
            Email = "anna.maria@example.com",
            Phone = "999999"
        };

        // ACT
        var updated = await _service.UpdateAsync(customer.Id, dto);


        // ASSERT
        Assert.IsNotNull(updated);
        Assert.That(updated!.FirstName, Is.EqualTo("Anna-Maria"));
        Assert.That(updated.Email, Is.EqualTo("anna.maria@example.com"));

        var dbCustomer = await _db.Customers.FindAsync(customer.Id);
        Assert.IsNotNull(dbCustomer);
        Assert.That(dbCustomer!.FirstName, Is.EqualTo("Anna-Maria"));
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllCustomers()
    {
        // ARRANGE – lägg till två kunder i databasen
        _db.Customers.AddRange(
            new Customer
            {
                CompanyId = 1,
                FirstName = "Kalle",
                LastName = "Karlsson",
                Email = "kalle@example.com"
            },
            new Customer
            {
                CompanyId = 1,
                FirstName = "Lisa",
                LastName = "Larsson",
                Email = "lisa@example.com"
            }
        );
        await _db.SaveChangesAsync();

        // ACT
        var result = await _service.GetAllAsync();

        // ASSERT
        Assert.IsNotNull(result);
        var list = result.ToList();
        Assert.That(list.Count, Is.EqualTo(2));
        Assert.That(list.Any(c => c.FirstName == "Kalle"));
        Assert.That(list.Any(c => c.FirstName == "Lisa"));
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnCustomer_WhenCustomerExists()
    {
        // ARRANGE – lägg till en kund i databasen
        var customer = new Customer
        {
            CompanyId = 1,
            FirstName = "Test",
            LastName = "Person",
            Email = "test.person@example.com",
            Phone = "555555"
        };

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        // ACT – hämta via service
        var result = await _service.GetByIdAsync(customer.Id);

        // ASSERT
        Assert.IsNotNull(result);
        Assert.That(result!.Id, Is.EqualTo(customer.Id));
        Assert.That(result.FirstName, Is.EqualTo("Test"));
        Assert.That(result.Email, Is.EqualTo("test.person@example.com"));
    }

    [Test]
    public async Task UpdateAsync_ShouldReturnNull_WhenCustomerDoesNotExist()
    {
        // ARRANGE – DTO för en kund som inte finns
        var dto = new UpdateCustomerRequest
        {
            Id = 999, // finns inte
            CompanyId = 1,
            FirstName = "Ghost",
            LastName = "Customer",
            Email = "ghost@example.com",
            Phone = "000000"
        };

        // ACT
        var result = await _service.UpdateAsync(dto.Id, dto);

        // ASSERT
        Assert.IsNull(result);
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoCustomers()
    {
        // ARRANGE – ingenting, DB är tom

        // ACT
        var result = await _service.GetAllAsync();

        // ASSERT
        Assert.IsNotNull(result);
        var list = result.ToList();
        Assert.That(list.Count, Is.EqualTo(0));
    }

}
