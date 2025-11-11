using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_z_backend.Entities;

public class RefreshToken
{
    [Key]
    public required Guid Id { set; get; }
    public required string Token { set; get; }
    public DateTime ExpiresAt { set; get; }
    public DateTime CreatedAt { set; get; } = DateTime.UtcNow;

    public Guid UserId { set; get; }
    [ForeignKey("UserId")]
    public virtual User User { set; get; } = null!;
}
