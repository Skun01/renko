using System;
using project_z_backend.DTOs.User;
using project_z_backend.Entities;

namespace project_z_backend.Mapping;

public static class UserMapping
{
    public static UserResponse ToResponse(this User user)
    {
        return new UserResponse(
            user.UserName,
            user.Email,
            user.CreatedAt,
            user.UpdatedAt,
            user.Roles.Select(r => r.Name).ToList()
        );
    }
}
