using System;
using System.ComponentModel.DataAnnotations;

namespace project_z_backend.Entities;

public class Role : BaseEntity
{
    public required string Name { set; get; }
}
