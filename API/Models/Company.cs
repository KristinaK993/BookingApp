using API.Models;

namespace API.Models;

public class Company
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public ICollection<Service> Services { get; set; } = new List<Service>();
    public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
