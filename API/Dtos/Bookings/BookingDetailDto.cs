namespace API.Dtos.Bookings;

public class BookingDetailDto
{
    public int Id { get; set; }

    public int CompanyId { get; set; }
    public int CustomerId { get; set; }
    public int EmployeeId { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

   
    public string Status { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public string CustomerName { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;

    public List<BookingServiceItemDto> Services { get; set; } = new();
}

public class BookingServiceItemDto
{
    public int ServiceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
}
