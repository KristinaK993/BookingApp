namespace API.Dtos.Bookings;

public class BookingCreateDto
{
    public int CompanyId { get; set; }
    public int CustomerId { get; set; }
    public int EmployeeId { get; set; }

    public string Status { get; set; } = "Booked";

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public string? Notes { get; set; }
    public List<int> ServiceIds { get; set; } = new();
}
