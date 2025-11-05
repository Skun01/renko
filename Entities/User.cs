using System;
using System.ComponentModel.DataAnnotations;

namespace project_z_backend.Entities;

public class User : BaseEntity
{
    public required string UserName { set; get; }
    public required string PasswordHash { set; get; }
    public required string Email { set; get; }
    public virtual Role? Role { set; get; }
}
