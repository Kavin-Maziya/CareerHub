using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using APIs.DTOs;

namespace APIs.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;

    // Hardcoded user store — simulates a database Users table.
    private static readonly (string Username, string Password, string Role)[] _users =
    [
        ("Employer",        "password123", "Employer"),

    ];

    // IConfiguration is injected so the JWT secret is read from appsettings,
  
    public AuthService(IConfiguration config)
    {
        _config = config;
    }

    public LoginResponse? Login(LoginRequest request)
    {
        // Find a matching user by username and password.
     
        var user = _users.FirstOrDefault(u =>
u.Username == request.Username && u.Password == request.Password);

        // Return null so the controller decides the HTTP response.
        if (user == default)
            return null;

        var token = BuildToken(user.Username, user.Role);
        return new LoginResponse(token);
    }

    // Constructs and signs the JWT
    private string BuildToken(string username, string role )
    {
        
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username), // Who the token belongs to
            new Claim(ClaimTypes.Role, role)                  // Which role gates they can pass
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2), 
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}