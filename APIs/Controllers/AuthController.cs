using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using APIs.DTOs;
using APIs.Services;

namespace APIs.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    // POST /api/auth/login
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var response = authService.Login(request);

        // Service returns null for any invalid credential combination.
        if (response is null)
            return Unauthorized(); // 401

        return Ok(response); // 200 — body: { "token": "eyJ..." }
    }

    // Returns the identity of the currently authenticated caller by reading the claims
    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        var username = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        var role     = User.FindFirstValue(ClaimTypes.Role);

        return Ok(new { username, role });
    }
}