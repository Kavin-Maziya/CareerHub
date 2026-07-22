using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using APIs.DTOs;
using APIs.Data;
using APIs.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace APIs.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;
    private readonly CareerHubDbContext _db;


    // IConfiguration is injected so the JWT secret is read from appsettings,
    // and reads the seeded users from the database
    public AuthService(IConfiguration config, CareerHubDbContext db)
    {
    _config = config;
    _db = db;
    }

    public LoginResponse? Login(LoginRequest request)
    {
        // Find a matching user by username and role.
     
       var user = _db.Users.FirstOrDefault(u =>
         u.Username == request.Username &&
         u.Role == request.Role &&
         u.IsActive);

       if (user is null)
         return null;

       if (user.PasswordHash != request.Password)
         return null;

      var token = BuildToken(user.Username, user.Role);
      var refreshToken = GenerateRefreshToken();
        
    _db.RefreshTokens.Add(new RefreshToken
      {
      Token = refreshToken,
      UserId = user.Id,
      User = user,
      ExpiresAt = DateTime.UtcNow.AddDays(30),
      });
      _db.SaveChanges();

        return new LoginResponse(token, refreshToken);
    }

    // Constructs and signs the JWT
    private string BuildToken(string username, string role)
    {
        
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username), // Who the token belongs to
            new Claim(ClaimTypes.Name, username),             // Matches NameClaimType in Program.cs
            new Claim(ClaimTypes.Role, role)                  // Which role gates they can pass
        };

        var secretKey = _config["Jwt:SecretKey"];
        if (string.IsNullOrEmpty(secretKey))
            throw new InvalidOperationException("JWT Secret Key is not configured.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(10), 
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    private static string GenerateRefreshToken()
    {
    return Convert.ToBase64String(
        RandomNumberGenerator.GetBytes(64)
    );
    }
}