using System;
using BCrypt.Net;
using project_z_backend.DTOs.Auth;
using project_z_backend.Entities;
using project_z_backend.Interfaces.Repositories;
using project_z_backend.Interfaces.Services;
using project_z_backend.Mapping;
using project_z_backend.Share;

namespace project_z_backend.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    public AuthService(IUserRepository userRepository)
    {
        _userRepo = userRepository;
    }
    public async Task<Result> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepo.GetByEmailAsync(request.Email);
        if (existingUser.IsSuccess)
            return Result.Failure(Error.Conflict("User email already exist"));

        // Create user:
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        User newUser = request.ToEntity(passwordHash);
        return await _userRepo.AddAsync(newUser);
    }
}
