namespace API.Dtos.Auth;

public class RegisterRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int CompanyId { get; set; }
}
