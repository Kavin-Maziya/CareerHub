namespace APIs.DTOs;
public record LoginRequest(
    string Username,
    string Password,
    string Role);