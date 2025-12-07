using System.ComponentModel.DataAnnotations;

namespace API.Models;

public class Service
{
    public int Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = null!;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Range(1, 600)]
    public int DurationMinutes { get; set; }

    [Range(0, 100000)]
    public decimal Price { get; set; }

    // Navigation
    public Company? Company { get; set; }
    public ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();
}
