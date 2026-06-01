namespace APIs.DTOs;
public record LoginRequest(
    string username,
    string Password);