using System;
using System.ComponentModel.DataAnnotations;

namespace project_z_backend.Options;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    [Required]
    public string Key { get; init; } = null!;
    
    [Required]
    public string Issuer { get; init; } = null!;

    [Required]
    public string Audience { get; init; } = null!;

    [Range(0.1, 72)]
    public int ExpireHours { get; init; }
    
    [Range(1, 72)]
    public int EmailTokenExpireHours { get; init; }
    [Range(1, 100)]
    public int RefreshTokenExpireDays { get; init; }
}
