using System;
using System.ComponentModel.DataAnnotations;

namespace project_z_backend.Options;

public class EmailSettings
{
    public const string SectionName = "EmailSettings";
    [Required]
    public string SmtpServer { get; init; } = null!;
    [Required]
    public int Port { get; init; }
    [EmailAddress]
    [Required]
    public string FromAddress { get; init; } = null!;
    [Required]
    public string Password { get; init; } = null!;
}
