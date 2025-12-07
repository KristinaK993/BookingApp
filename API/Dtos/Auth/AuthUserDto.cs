namespace API.Dtos.Auth;

public class AuthUserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string? Name { get; set; }
    public int CompanyId { get; set; }
}
