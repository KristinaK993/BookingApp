using System.ComponentModel.DataAnnotations;

namespace API.Models;

public class Booking
{
    public int Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    public int CustomerId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    // Lägg till detta:
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Pending";

    // Navigation
    public Company? Company { get; set; }
    public Customer? Customer { get; set; }
    public Employee? Employee { get; set; }

    public ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();
}
