namespace API.Dtos.Bookings;

public class BookingUpdateDto
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

   
    public string Status { get; set; } = "Booked";

    public string? Notes { get; set; }

    public List<int> ServiceIds { get; set; } = new();
}
