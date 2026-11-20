using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ConstructionManagement.BLL.DTOs;
using ConstructionManagement.DAL.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

// Replace User with your actual scaffolded entity class name.
using UserEntity = ConstructionManagement.DAL.Entities.User;

namespace ConstructionManagement.BLL.Services;

public class AuthService
{
    private readonly ConstructionDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<UserEntity> _passwordHasher = new();

    public AuthService(
        ConstructionDbContext db,
        IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        
        var user = await _db.Users
       .FirstOrDefaultAsync(u =>
           u.Phone == request.Phone &&
           u.StatusId == 1 &&
           u.IsActive);
        if (user == null)
            return null;

        var verification = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password);

        if (verification == PasswordVerificationResult.Failed)
            return null;

        var jwt = _configuration.GetSection("Jwt");
        var key = jwt["Key"]
            ?? throw new InvalidOperationException("JWT key is missing.");

        if (Encoding.UTF8.GetByteCount(key) < 32)
            throw new InvalidOperationException(
                "JWT key must contain at least 32 bytes.");

        var expiresAt = DateTime.UtcNow.AddHours(2);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(
            signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new LoginResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            UserId = user.UserId,
            FullName = user.FullName,
            Role = user.Role,
            ExpiresAt = expiresAt
        };
    }

    public async Task<RegisterResponse> RegisterAsync(
    RegisterRequest request)
    {
        var allowedRoles = new[]
        {
        "Admin",
        "Builder",
        "Supervisor",
        "Contractor"
    };

        if (!allowedRoles.Contains(request.Role))
            throw new ArgumentException("Invalid user role.");

        var existingUser = await _db.Users
            .AnyAsync(u => u.Phone == request.Phone);

        if (existingUser)
            throw new InvalidOperationException(
                "Phone number already registered.");

        var user = new UserEntity
        {
            FullName = request.FullName,
            Phone = request.Phone,
            Email = request.Email,
            Role = request.Role,
            StatusId = 1, // Active
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            request.Password
        );

        _db.Users.Add(user);

        await _db.SaveChangesAsync();

        return new RegisterResponse
        {
            UserId = user.UserId,
            FullName = user.FullName,
            Phone = user.Phone,
            Email = user.Email,
            Role = user.Role,
            Message = "User registered successfully."
        };
    }
}