using System;
using System.ComponentModel.DataAnnotations;

namespace project_z_backend.Entities;

public class BaseEntity
{
    [Key]
    public Guid Id { set; get; } = Guid.NewGuid();
    public DateTime CreatedAt { set; get; }
    public DateTime UpdatedAt { set; get; }
}
