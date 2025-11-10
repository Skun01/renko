using System;
using project_z_backend.DTOs.Auth;
using project_z_backend.DTOs.User;
using project_z_backend.Share;

namespace project_z_backend.Interfaces.Services;

public interface IAuthService
{
    Task<Result> RegisterAsync(RegisterRequest request);
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
    Task<Result<UserResponse>> GetCurrentUserLoginAsync(HttpContext httpContext);
    Task<Result> VerifyEmailAsync(string token);

}
