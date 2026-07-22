namespace APIs.DTOs;

public record LoginResponse( string AccessToken, string RefreshToken, UserResponse User); 