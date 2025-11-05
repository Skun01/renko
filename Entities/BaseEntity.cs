using System;
using System.ComponentModel.DataAnnotations;

namespace project_z_backend.Entities;

public class BaseEntity
{
    [Key]
    public required Guid Id { set; get; }
    public DateTime CreatedAt { set; get; } = DateTime.Now;
    public DateTime UpdatedAt { set; get; }
}
