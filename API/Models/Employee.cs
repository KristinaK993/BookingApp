using API.Models;

namespace API.Models;

public class Employee
{
    public int Id { get; set; }

    public int CompanyId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;

    public Company? Company { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
