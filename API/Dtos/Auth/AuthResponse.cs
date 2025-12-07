namespace API.Dtos.Auth;

public class AuthResponse
{
    public AuthUserDto User { get; set; } = null!;
    public string Token { get; set; } = null!;
}
