namespace APIs.DTOs;

public record UserResponse(
    Guid Id,
    string Username,
    string Role
);