namespace API.Models;

public class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? Name { get; set; }

    public int CompanyId { get; set; }
    public Company? Company { get; set; }

   
}
