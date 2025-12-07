using System.ComponentModel.DataAnnotations;

namespace API.Models;

public class Customer
{
    public int Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = null!;

    [EmailAddress]
    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    // Navigation
    public Company? Company { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
