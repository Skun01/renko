using System;
using project_z_backend.DTOs.Auth;
using project_z_backend.Share;

namespace project_z_backend.Interfaces.Services;

public interface IAuthService
{
    Task<Result> RegisterAsync(RegisterRequest request);
}
