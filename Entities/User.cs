using System;
using System.ComponentModel.DataAnnotations;

namespace project_z_backend.Entities;

public class User : BaseEntity
{
    public required string UserName { set; get; }
    public required string PasswordHash { set; get; }
    public required string Email { set; get; }
    public bool IsEmailVerified { set; get; } = false;
    public virtual List<Role> Roles { set; get; } = [];
    public virtual List<RefreshToken> RefreshTokens { set; get; } = [];
}
