using System;
using System.ComponentModel.DataAnnotations;

namespace project_z_backend.Options;

public class FrontendSettings
{
    public const string SectionName = "FrontendSettings";
    [Required]
    public string BaseUrl { get; init; } = null!;
    [Required]
    public string VerifyEmailPath { get; init; } = null!;
}
