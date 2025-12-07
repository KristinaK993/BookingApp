using API.Data;
using API.Dtos.Auth;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IJwtTokenService _jwt;
    private readonly PasswordHasher<User> _hasher = new();

    public AuthController(AppDbContext db, IJwtTokenService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    // POST: api/auth/register
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        if (await _db.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest("Email already in use.");

        var company = await _db.Companies.FindAsync(request.CompanyId);
        if (company == null)
            return BadRequest("Invalid CompanyId.");

        var user = new User
        {
            Email = request.Email,
            Name = request.Name,
            CompanyId = request.CompanyId
        };

        user.PasswordHash = _hasher.HashPassword(user, request.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = _jwt.GenerateToken(user);

        var userDto = new AuthUserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            CompanyId = user.CompanyId
        };

        return Ok(new AuthResponse
        {
            User = userDto,
            Token = token
        });
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
            return BadRequest("Invalid credentials.");

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
            return BadRequest("Invalid credentials.");

        var token = _jwt.GenerateToken(user);

        var userDto = new AuthUserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            CompanyId = user.CompanyId
        };

        return Ok(new AuthResponse
        {
            User = userDto,
            Token = token
        });
    }
}

