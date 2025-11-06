namespace project_z_backend.DTOs.Auth;

public record class RegisterRequest(
    string Username,
    string Email,
    string Password
);