using System;
using project_z_backend.DTOs.Auth;
using project_z_backend.Entities;

namespace project_z_backend.Mapping;

public static class AuthMapping
{
    public static User ToEntity(this RegisterRequest request, string passwordHash)
    {
        return new User
        {
            UserName = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            IsEmailVerified = false
        };
    } 
}
