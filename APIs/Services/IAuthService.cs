using APIs.DTOs;

namespace APIs.Services;

// Defines the contract for authentication.

public interface IAuthService
{
    // Returns a LoginResponse with a signed JWT on success.
    // Returns null if the credentials are invalid — the controller turns null into a 401.
    LoginResponse? Login(LoginRequest request);

    LoginResponse? Refresh(string refreshToken);
}