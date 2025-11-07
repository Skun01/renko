namespace project_z_backend.DTOs.User;

public record class UserResponse(
    string Username,
    string Email,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<string> Roles
);