using System;
using System.ComponentModel.DataAnnotations;

namespace project_z_backend.Options;

public class AppSettings
{
    public const string SectionName = "App";
    [Required]
    public string Name { get; init; } = null!;
    [EmailAddress]
    [Required]
    public string SupportEmail { get; init; } = null!;
}
